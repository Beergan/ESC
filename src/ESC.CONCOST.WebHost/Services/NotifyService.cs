using Microsoft.AspNetCore.Mvc;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace ESC.CONCOST.WebHost;

public class NotifyService : INotifyService
{
    private readonly ILogger<NotifyService> _log;
    private readonly IServiceProvider _serviceProvider;
    private const int _notifyPoolSize = 50; 

    public NotifyService(ILogger<NotifyService> log, IServiceProvider serviceProvider)
    {
        _log = log;
        _serviceProvider = serviceProvider;
    }

    [HttpPost]
    public async Task<Result> AddNotify(string content, string href, Guid? targetUserGuid = null)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var myContext = scope.ServiceProvider.GetRequiredService<IMyContext>();
            
            var entity = new EntityNotification
            {
                Guid = Guid.NewGuid(),
                Guid_Notification = Guid.NewGuid(),
                TitleVi = content,
                TitleEn = content,
                Href = href,
                Avatar = "",
                Guid_User = targetUserGuid ?? Guid.Empty,
                Guid_UserNotification = Array.Empty<Guid>(),
                DateCreated = DateTime.UtcNow,
                UserCreated = "System",
                Check = false,
                Module = ""
            };

            using ( var db = myContext.ConnectDb())
            {
             await db.Repo<EntityNotification>().Insert(entity);

            }
            MyContext.PublishEventStatic("GOT_NEW_NOTIFICATION");
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error in AddNotify");
            return Result.Error(ex.Message);
        }
    }

    [HttpGet]
    public async Task<ResultsOf<ModelNotification>> GetNotifies([FromQuery] Guid? userGuid = null)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var myContext = scope.ServiceProvider.GetRequiredService<IMyContext>();
            var currentEmpGuid = userGuid ?? myContext.GuidEmployee;


            using var db = myContext.ConnectDb();
            var list = await db.Set<EntityNotification>()
                .Where(x => x.Guid_User == Guid.Empty || x.Guid_User == currentEmpGuid)
                .OrderByDescending(x => x.DateCreated)
                .Take(_notifyPoolSize)
                .Select(x => new ModelNotification
                {
                    Message = x.TitleVi,
                    Href = x.Href,
                    NotityTime = x.DateCreated,
                    TargetUserGuid = x.Guid_User == Guid.Empty ? (Guid?)null : x.Guid_User
                })
                .ToListAsync();

            return list;
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Error in GetNotifies");
            return new List<ModelNotification>();
        }
    }
}
