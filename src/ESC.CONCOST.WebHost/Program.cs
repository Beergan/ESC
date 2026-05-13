using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Components.WebAssembly.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using Newtonsoft.Json;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using ESC.CONCOST.Db;
using ESC.CONCOST.WebApp;
using ESC.CONCOST.WebHost;
using ESC.CONCOST.WebHost.Classes;
using ESC.CONCOST.WebHost.Services;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configs = builder.Configuration;

if (!builder.Environment.IsDevelopment())
{
    //builder.WebHost.UseSentry(options =>
    //{
    //    options.StackTraceMode = StackTraceMode.Enhanced;
    //    options.ServerName = configs.GetValue<string>("Sentry:ServerName");
    //    options.Release = AutoClass.FullVersion + " " + AutoClass.BuildName;
    //});
}

var dbType = configs.GetValue<string>("AppSettings:DbType");
switch (dbType)
{
    case "Mssql":
        DbMssqlRegister.ConfigureServices(services, configs, (builder) =>
        {
            var entities = AssembliesUtil.GetAspNetAssemblies().GetInstances<IEntityRegister>();
            foreach (var i in entities)
            {
                i.RegisterEntities(builder);
            }
        });
        break;

    case "Mysql":
        DbMysqlRegister.ConfigureServices(services, configs, (builder) =>
        {
            var entities = AssembliesUtil.GetAspNetAssemblies().GetInstances<IEntityRegister>();
            foreach (var i in entities)
            {
                i.RegisterEntities(builder);
            }
        });
        break;

    case "Postgres":
        DbPostgresRegister.ConfigureServices(services, configs, (builder) =>
        {
            var entities = AssembliesUtil.GetAspNetAssemblies().GetInstances<IEntityRegister>();
            foreach (var i in entities)
            {
                i.RegisterEntities(builder);
            }
        });
        break;
    case "Memory":
        DbMemoryRegister.ConfigureServices(services, configs, (builder) =>
        {
            var entities = AssembliesUtil.GetAspNetAssemblies().GetInstances<IEntityRegister>();
            foreach (var i in entities)
            {
                i.RegisterEntities(builder);
            }
        });
        break;
}

services.AddScoped<Func<IDbContext>>((provider) => () => provider.GetRequiredService<IDbContext>());
services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
services.AddScoped(typeof(ICacheRepository<>), typeof(CacheRepository<>));
services.AddScoped<IMyContext, MyContext>();

services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtToken:Audience"],
        ValidIssuer = builder.Configuration["JwtToken:Issuer"],
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtToken:SigningKey"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/NotifyHub"))
            {
                context.Token = accessToken;
            }
            else if (context.Request.Cookies.ContainsKey("Auth"))
            {
                string token = context.Request.Cookies["Auth"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var claims = context.Principal.Claims;
            var tokenUserName = context.Principal.Identity?.Name;


            var versionSvc = context.HttpContext.RequestServices.GetRequiredService<IPermissionVersionService>();

            // 2. Check User Version
            var tokenUserVersionStr = claims.FirstOrDefault(x => x.Type == "UserVersion")?.Value;
            if (int.TryParse(tokenUserVersionStr, out int tokenUserVersion))
            {
                if (versionSvc.GetUserVersion(tokenUserName) != tokenUserVersion)
                {
                    context.Fail("User permission version mismatch. Revoking token.");
                    return Task.CompletedTask;
                }
            }

            // 2. Check Role Versions
            var roleVersionClaims = claims.Where(x => x.Type.StartsWith("RoleVersion_"));
            foreach (var claim in roleVersionClaims)
            {
                var roleId = claim.Type.Replace("RoleVersion_", "");
                if (int.TryParse(claim.Value, out int tokenRoleVersion))
                {
                    if (versionSvc.GetRoleVersion(roleId) != tokenRoleVersion)
                    {
                        context.Fail("Role permission version mismatch. Revoking token.");
                        return Task.CompletedTask;
                    }
                }
            }

            return Task.CompletedTask;
        },
        OnChallenge = async (context) =>
        {
            context.HandleResponse();

            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                if (context.HttpContext.Request.Path.Value.StartsWith("/api"))
                {
                    context.Response.StatusCode = 200;
                    context.Response.ContentType = "application/json";
                    await context.HttpContext.Response.WriteAsync("{\"success\":false,\"message\":\"Xác thực thất bại!\"}");
                }
                else
                {
                    context.Response.Redirect("/login");
                }
            }
        }
    };
});

services.AddHttpContextAccessor();
services.AddHttpClient();
services.AddScoped(sp =>
{
    var nav = sp.GetRequiredService<NavigationManager>();
    var client = new HttpClient { BaseAddress = new Uri(nav.BaseUri) };

    try
    {
        var accessor = sp.GetRequiredService<IHttpContextAccessor>();
        var context = accessor.HttpContext;
        if (context != null && context.Request.Cookies.ContainsKey("Auth"))
        {
            var token = context.Request.Cookies["Auth"];
            client.DefaultRequestHeaders.Add("Cookie", $"Auth={token}");
        }
    }
    catch { }

    return client;
});

builder.Services
    .AddControllersWithViews()
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddRazorPages()
.AddDataAnnotationsLocalization(o =>
{
    o.DataAnnotationLocalizerProvider = (t, f) => new MyStringLocalizer(services);
});
builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(o =>
    {
        o.DetailedErrors = true;
        o.DisconnectedCircuitRetentionPeriod = TimeSpan.FromHours(1);
    });
// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(o =>
    {
        // Keep the circuit recoverable for up to 1 hour, while still sending
        // regular keep-alive frames so proxies/load balancers don't drop the socket.
        o.ClientTimeoutInterval = TimeSpan.FromHours(1);
        o.HandshakeTimeout = TimeSpan.FromSeconds(30);
        o.KeepAliveInterval = TimeSpan.FromSeconds(30);
    })
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddSignalR();

services.AddScoped<ISessionId, SessionId>();
services.AddScoped<IAuthService, ServerAuthService>();
//services.AddScoped<HostAuthenticationStateProvider>();
//services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<HostAuthenticationStateProvider>());
builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

services.AddMudServices();
services.AddScoped<LazyAssemblyLoader>();
services.AddScoped<ISweetAlertService, SweetAlertServerService>();
services.AddScoped<IMyCookie, MyCookieServer>();
services.AddSingleton(typeof(FieldExtractor<>));
services.AddSingleton<IPermissionVersionService, PermissionVersionService>();
services.AddScoped<ITextTranslator, TextTranslatorForServer>();
services.AddScoped<INotifyService, NotifyService>();
services.AddScoped<IBlazorContext, MyContext>();
services.AddScoped<IServiceBase, MyServiceBase>();
services.AddScoped<IMailSettingService, MailSettingService>();
services.AddScoped<ESC.CONCOST.Abstract.LayoutService>();
services.AddScoped<IBackgroundUploadService, BackgroundUploadService>();

//services.AddScoped<IMediatorHangfireBridge, MediatorHangfireBridge>();
services.AddHangfire((sp, config) =>
{
    config.UseMemoryStorage();
    config.UseSerializerSettings(new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
});
services.AddHangfireServer();

services.Configure<CacheConfiguration>(configs.GetSection("CacheConfiguration"));
services.AddMemoryCache();

services.AddTransient<MemoryCacheService>();
//services.AddTransient<RedisCacheService>();

services.AddTransient<Func<CacheMode, ICacheService>>(serviceProvider => key =>
{
    switch (key)
    {
        case CacheMode.Memory:
            return serviceProvider.GetRequiredService<MemoryCacheService>();
        //case CacheTech.Redis:
        //    return serviceProvider.GetService<RedisCacheService>();
        default:
            return serviceProvider.GetRequiredService<MemoryCacheService>();
    }
});

var assemblies = AssembliesUtil.GetAspNetAssemblies();
var aspnetModules = assemblies.GetInstances<IModuleAspNet>();
foreach (var module in aspnetModules)
{
    module.ConfigureServices(services, configs);
}
services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
});
services.Configure<MailSettings>(configs.GetSection("MailSettings"));
services.AddScoped<IEmailRenderer, RazorEmailRenderer>();

string SwaggerEnabled = configs.GetValue<string>("SwaggerEnabled", "");

if (SwaggerEnabled.ToLower() == "true")
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Version = "v1",
            Title = "ERP Workshop",
            Description = "Hệ thông quản lý nhân sự",
        });

        options.AddJsonQuerySupport();

        options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
        {
            new OpenApiSecurityScheme
            {
                Name = "Bearer",
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
        });

        var aspnetssemblies = AssembliesUtil.GetAspNetAssemblies();
        foreach (var module in aspnetssemblies)
        {
            options.IncludeXmlComments(module.Location.Replace(".dll", "Core.xml"), includeControllerXmlComments: true);
            options.IncludeXmlComments(module.Location.Replace(".dll", ".xml"), includeControllerXmlComments: true);
        }

        var xmlHostPath = Path.Combine(System.AppContext.BaseDirectory, "ESC.CONCOST.WebHost.xml");
        options.IncludeXmlComments(xmlHostPath, includeControllerXmlComments: true);
    });
}

//builder.Services.AddCors(policyBuilder =>
//    policyBuilder.AddDefaultPolicy(policy =>
//        policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader())
//);
builder.Services.AddCors(policyBuilder =>

    policyBuilder.AddDefaultPolicy(policy =>

        policy.AllowAnyOrigin()

              .AllowAnyHeader()

              .AllowAnyMethod())

);
builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 1024 * 1024 * 100; // 100MB for SignalR messages (not file chunks, but just in case)
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null; // Unlimited for chunked upload
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = 10L * 1024 * 1024 * 1024; // 10GB
    x.MemoryBufferThreshold = int.MaxValue;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();  // ← Add this
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/switch-to-wasm", (HttpContext ctx) =>
{
    ctx.Response.Cookies.Delete("blazorMode");
    ctx.Response.Cookies.Append("blazorMode", "wasm", new CookieOptions { Expires = DateTime.UtcNow.AddDays(30) });
    ctx.Response.Redirect(ctx.Request.Query["redirect"]!);
});

app.MapGet("/switch-to-server", (HttpContext ctx) =>
{
    ctx.Response.Cookies.Delete("blazorMode");
    ctx.Response.Cookies.Append("blazorMode", "server", new CookieOptions { Expires = DateTime.UtcNow.AddDays(30) });
    ctx.Response.Redirect(ctx.Request.Query["redirect"]!);
});

if (SwaggerEnabled.ToLower() == "true")
{

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
        options.DocExpansion(DocExpansion.None);
        options.ConfigObject.AdditionalItems.Add("tagsSorter", "alpha");
    });
}
System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("vi-VN");
System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("vi-VN");
// Cấu hình các endpoint
//app.MapBlazorHub(options =>
//{
//    options.WebSockets.CloseTimeout = new TimeSpan(1, 1, 1);
//    options.LongPolling.PollTimeout = new TimeSpan(1, 0, 0);
//});

app.MapBlazorHub();
app.MapHub<NotifyHub>("/NotifyHub");
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToPage("/_Host");

foreach (var module in aspnetModules)
{
    module.BuildModule(app);
}


await app.Services.CreateOrUpdateTernant("localhost");






app.Services.MakeMeCanResolveFromStatic();

app.UseCors();

app.Run();
