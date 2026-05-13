using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.Db;
using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Claims;

namespace ESC.CONCOST.WebHost;

public class MyContext : IMyContext, IBlazorContext
{
    public MyContext(IServiceProvider provider,
        Func<IDbContext> dbFactory,
        IHttpContextAccessor accessor,
        Func<CacheMode, ICacheService> cacheMode,
        IMyCookie cookie,
        INotifyService notifier,
        ISessionId sessionId
        )
    {
        _provider = provider;
        _dbFactory = dbFactory;
        _accessor = accessor;
        _cacheMode = cacheMode;
        _cookie = cookie;
        _notifier = notifier;
        _sessionId = sessionId;
    }

    [JsonIgnore]
    private Func<CacheMode, ICacheService> _cacheMode;
    [JsonIgnore]
    private Func<IDbContext> _dbFactory;
    [JsonIgnore]
    private readonly IServiceProvider _provider;
    [JsonIgnore]
    private readonly IHttpContextAccessor _accessor;
    [JsonIgnore]
    private readonly IMyCookie _cookie;
    [JsonIgnore]
    private readonly INotifyService _notifier;
    [JsonIgnore]
    private readonly ISessionId _sessionId;

    [JsonIgnore]
    public HttpContext HttpContext => _accessor?.HttpContext;

    public IDbContext ConnectDb()
    {
        var dbContext = _dbFactory.Invoke();

        dbContext.IpAddress = this.IpAddress;
        dbContext.UserId = this.UserId;
        dbContext.EmployeeGuid = this.GuidEmployee;

        return dbContext;
    }

    private IDbContext _db;
    [JsonIgnore]
    public IDbContext Db => _db ?? (_db = _dbFactory.Invoke());

    public IDatabaseTransaction BeginTransaction()
    {
        var dbContext = _dbFactory.Invoke();
        dbContext.IpAddress = IpAddress;
        dbContext.UserId = UserId;
        dbContext.EmployeeGuid = GuidEmployee;

        return new EntityDatabaseTransaction(dbContext);
    }

    public IDatabaseTransaction BeginTransaction(string userId)
    {
        var dbContext = _dbFactory.Invoke();
        dbContext.IpAddress = "PL-TOS";
        dbContext.UserId = userId;
        dbContext.EmployeeGuid = GuidEmployee;

        return new EntityDatabaseTransaction(dbContext);
    }


    public DbSet<T> Set<T>() where T : class
    {
        return Db.Set<T>();
    }

    public IRepository<T> Repo<T>() where T : class
    {
        return new BaseRepository<T>(Db);
    }

    public ICacheRepository<T> Cache<T>() where T : class
    {
        return new CacheRepository<T>(Db, _cacheMode);
    }

    public bool CheckPermission<T>(params T[] requiredClaims) where T : Enum
    {
        ClaimsPrincipal user = null;
        try {
            user = HttpContext?.User;
        } catch { /* Handle Disposed FeatureCollection */ }

        if (user == null) return false;
        IEnumerable<Claim> userClaims = user.Claims;

        // Admin bypass
        if (userClaims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Admin")) return true;

        var featureAttrb = requiredClaims[0].GetType().GetCustomAttribute<FeatureAttribute>();
        if (featureAttrb == null) return false;

        long requiredPermission = Convert.ToInt64(requiredClaims.Select(x => (long)Math.Pow(2, Convert.ToInt64(x))).Sum());
        List<long> availablePermissionLst = userClaims
            .Where(x => x.Type == featureAttrb.Name)
            .Select(x => Convert.ToInt64(x.Value))
            .ToList();
        foreach (var availablePermission in availablePermissionLst)
        {
            if (availablePermission == 0) continue;

            if ((availablePermission & requiredPermission) == requiredPermission) return true;
        }
        return false;
    }

    public string IpAddress => HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "NA";

    public string UserAgent => HttpContext?.Request?.Headers["User-Agent"] ?? "NA";

    public string UserId => HttpContext?.User?.Identity?.Name ?? "NA";

    public string EnterpriseCode => HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "EnterpriseCode")?.Value ?? "MPD";

    public SA_USER GetCurrentUser()
    {
        using (var db = this.ConnectDb())
        {
            return db.Set<SA_USER>().AsNoTracking().FirstOrDefault(x => x.UserName == UserId);
        }
    }

    public T GetService<T>()
    {
        return _provider.GetService<T>();
    }

    [JsonIgnore]
    public ITextTranslator Text => _provider.GetRequiredService<ITextTranslator>();

    [JsonIgnore]
    public IMediator Mediator => _provider.GetRequiredService<IMediator>();

    //[JsonIgnore]
    //public IWorkFlowService Workflow => _provider.GetRequiredService<IWorkFlowService>();

    [JsonIgnore]
    public string Summary => JsonConvert.SerializeObject(this);

    [JsonIgnore]
    public INotifyService Notifier => _notifier;

    public event Action<string, string[]> OnNotify;

    public static Task ProcessEvent(string sessionId, string evt, params string[] data)
    {
        return Task.CompletedTask;
    }

    void IMyContext.PublishEventStatic(string evt, params string[] data)
    {
        MyContext.PublishEventStatic(evt, data);
    }

    public static void PublishEventStatic(string evt, params string[] data)
    {
        var hub = StaticResolver.Resolve<IHubContext<NotifyHub>>();
        if ((evt == "REFRESH_PERMISSION" || evt == "FORCE_LOGOUT") && data.Length > 0)
        {
            // Send only to users in that role
            hub.Clients.Group(data[0]).SendAsync("Notify", evt, data);
        }
        else if ((evt == "REFRESH_PERMISSION_USER" || evt == "FORCE_LOGOUT_USER") && data.Length > 0)
        {
            // Send only to that specific user
            hub.Clients.Group(data[0]).SendAsync("Notify", evt, data);
        }
        else
        {
            // Broadcast for everything else
            hub.Clients.All.SendAsync("Notify", evt, data);
        }
    }

    public Task PublishEvent(string evt, params string[] data)
    {
        return MyContext.ProcessEvent(_sessionId.Value.ToString(), evt, data);
    }

    private Func<string> _pageTitle;
    [JsonIgnore]
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
        StateChanged?.Invoke(evt);
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


    public Guid GuidEmployee
    {
        get
        {
            var value = HttpContext?.User?.Claims
                ?.FirstOrDefault(c => c.Type == nameof(GuidEmployee))
                ?.Value;

            return Guid.TryParse(value, out var guid)
                ? guid
                : Guid.Empty;
        }
    }

    public Guid GuidCompany
    {
        get
        {
            var value = HttpContext?.User?.Claims
                ?.FirstOrDefault(c => c.Type == nameof(GuidCompany))
                ?.Value;

            return Guid.TryParse(value, out var guid)
                ? guid
                : Guid.Empty;
        }
    }
}