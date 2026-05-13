using Microsoft.AspNetCore.Components.Authorization;
using ESC.CONCOST.Abstract;
using System.Security.Claims;

namespace ESC.CONCOST.WebApp;

public class HostAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService api;

    public HostAuthenticationStateProvider(IAuthService api)
    {
        this.api = api;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var identity = new ClaimsIdentity();
        try
        {
            var userModel = await GetCurrentUser();
            if (userModel.IsAuthenticated)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, userModel.UserName) }
                    .Concat(userModel.Claims.Select(c => new Claim(c.Key, c.Value)));

                identity = new ClaimsIdentity(claims, nameof(HostAuthenticationStateProvider));
            }
        }
        catch (HttpRequestException)
        {
        }

        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    private async Task<InfoUser> GetCurrentUser()
    {
        return await api.ValidateTokenInCookie();
    }

    public async Task<string> Login(LoginRequest model)
    {
        var result = await api.Login(model);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return "OK";
    }

    public async Task<string> Logout()
    {
        var result = await api.Logout();
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        return "OK";
    }

    public void NotifyRefresh()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}