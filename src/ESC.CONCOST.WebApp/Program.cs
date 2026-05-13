using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor.Services;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.WebApp;
using RestEase.HttpClientFactory;
using RonSijm.Blazyload;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.UseBlazyload();
builder.Services.AddMudServices();

builder.Services.AddHttpClient();
builder.Services.ConfigureHttpClientDefaults(o =>
{
    o.ConfigureHttpClient(c =>
        c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
});

builder.Services.AddSingleton<IAuthService, WasmAuthService>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<HostAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<HostAuthenticationStateProvider>());

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<HubConnection>(sp =>
{
    var navigationManager = sp.GetRequiredService<NavigationManager>();
    var cookie = sp.GetRequiredService<IMyCookie>();
    
    return new HubConnectionBuilder()
      .WithUrl(navigationManager.ToAbsoluteUri("/NotifyHub"), options => {
          options.AccessTokenProvider = async () => {
              return await cookie.GetCookie("Auth");
          };
      })
      .WithAutomaticReconnect()
      .Build();
});

builder.Services.AddRestEaseClient<INotifyService>(builder.HostEnvironment.BaseAddress);
builder.Services.AddScoped<ISweetAlertService, SweetAlertWasmService>();
builder.Services.AddScoped<IMyCookie, MyCookieWasm>();
builder.Services.AddSingleton(typeof(FieldExtractor<>));
builder.Services.AddScoped<IBlazorContext, BlazorContextWasm>();
builder.Services.AddScoped<ESC.CONCOST.Abstract.LayoutService>();

var cookie = builder.Services.BuildServiceProvider().GetRequiredService<IMyCookie>();
string langId = await cookie.GetCookie(nameof(langId), def: "ko");
builder.Services.AddScoped<ITextTranslator>(sp => new TextTranslatorForWasm(cookie, langId));


var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
var apiBaseUri = new Uri($"{baseUri}api/");

AppStatic.BaseAddress = builder.HostEnvironment.BaseAddress;

await builder.Build().RunAsync();