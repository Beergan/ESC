using Microsoft.AspNetCore.SignalR.Client;
using System.Reflection;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.WebApp;

public class BlazorContextWasm : IBlazorContext
{
    private CancellationTokenSource _cts = new CancellationTokenSource();
    private readonly IAuthService _authSvc;
    private readonly HubConnection _hubConn;
    private readonly IMyCookie _cookie;
    private readonly INotifyService _notifier;

    public bool CheckPermission<T>(params T[] requiredClaims) where T : Enum
    {
        var user = _authSvc.CurrentUser;
        if (user == null || !user.IsAuthenticated || user.Claims == null) return false;

        // Admin bypass - check both standard URI and short names
        if (user.Claims.Any(c => (c.Key == System.Security.Claims.ClaimTypes.Role || c.Key == "Role" || c.Key == "role") && c.Value == "Admin")) return true;

        var featureAttrb = requiredClaims[0].GetType().GetCustomAttribute<FeatureAttribute>();
        if (featureAttrb == null) return false;

        long requiredPermission = Convert.ToInt64(requiredClaims.Select(x => (long)Math.Pow(2, Convert.ToInt64(x))).Sum());
        
        var availablePermissionClaim = user.Claims.FirstOrDefault(x => x.Key == featureAttrb.Name);
        if (availablePermissionClaim == null) return false;

        if (long.TryParse(availablePermissionClaim.Value, out long availablePermission))
        {
            if (availablePermission == 0) return false;
            return (availablePermission & requiredPermission) == requiredPermission;
        }

        return false;
    }

    public BlazorContextWasm(IAuthService authSvc, HubConnection hubConn, IMyCookie cookie, INotifyService notify)
    {
        _authSvc = authSvc;
        _hubConn = hubConn;
        _cookie = cookie;
        _notifier = notify;

        _hubConn.Closed += async (error) => { 
            Console.WriteLine("SignalR Connection Closed. Retrying...");
            await ConnectWithRetryAsync(_cts.Token); 
        };
        _hubConn.On<string, string[]>("Notify", (evt, data) => {
            Console.WriteLine($"SignalR Received Event: {evt}");
            OnNotify?.Invoke(evt, data);
        });

#pragma warning disable CS4014
        this.ConnectWithRetryAsync(_cts.Token);
#pragma warning restore CS4014
    }

    private async Task<bool> ConnectWithRetryAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine("SignalR Connecting...");
                await _hubConn.StartAsync(token);
                Console.WriteLine("SignalR Connected Successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SignalR Connection Failed: {ex.Message}. Retrying in 5s...");
                await Task.Delay(5000, token);
            }
        }
        return false;
    }

    public Task PublishEvent(string evt, params string[] data)
    {
        return Task.CompletedTask;
    }

    public Task NotifyEvent(string evt, params string[] data)
    {
        return _hubConn.SendAsync("NotifyToServer", evt, data);
    }

    public event Action<string, string[]>? OnNotify;

    private Func<string> _pageTitle;
    public Func<string> PageTitle
    {
        get { return _pageTitle; }
        set
        {
            _pageTitle = value;
            NotifyStateChanged();
        }
    }

    public async Task<string> GetAuthToken()
    {
        return await _cookie.GetCookie("Auth");
    }

    public event Action<object[]> StateChanged;

    public void NotifyStateChanged(params object[] evt)
    {
        StateChanged.Invoke(evt);
    }

    private string _theme = null;
    public async Task<string> GetTheme()
    {
        return _theme ?? (_theme = await _cookie.GetCookie("ThemeId", ""));
    }

    public async Task<string> SetTheme(string themeId)
    {
        _theme = themeId;
        await _cookie.SetCookie("ThemeId", themeId, 30);
        NotifyStateChanged();
        return themeId;
    }


    public INotifyService Notifier => _notifier;
}