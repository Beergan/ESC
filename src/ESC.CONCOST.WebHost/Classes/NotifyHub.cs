using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ESC.CONCOST.WebHost;

[Authorize]
public class NotifyHub : Hub
{
    public Task NotifyToServer(string evt, string[] data)
    {
        return MyContext.ProcessEvent(Context.ConnectionId, evt, data);
    }

    public override async Task OnConnectedAsync()
    {
        var roleIds = Context.User?.Claims.Where(c => c.Type == "RoleId").Select(x => x.Value).ToList();
        if (roleIds != null)
        {
            foreach (var roleId in roleIds)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, roleId);
            }
        }

        var userName = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userName))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}