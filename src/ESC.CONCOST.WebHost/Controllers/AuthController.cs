using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;

namespace ESC.CONCOST.WebHost.Controllers;

/// <summary>
/// QUẢN LÝ XÁC THỰC
/// </summary>
[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly ServerAuthService _authService;

    public AuthController(
        IHttpContextAccessor httpContextAccessor,
        UserManager<SA_USER> userManager,
        SignInManager<SA_USER> signInManager,
        RoleManager<IdentityRole> roleManager,
        IServiceProvider svcProvider,
        IMyCookie cookie,
        IPermissionVersionService versionSvc,
        IConfiguration config,
        IMyContext ctx
    )
    {
        _authService = new ServerAuthService(
            httpContextAccessor,
            userManager,
            signInManager,
            roleManager,
            svcProvider,
            cookie,
            versionSvc,
            config,
            ctx
        );
    }

    [HttpPost("logout")]
    public async Task<IActionResult> LogoutApi()
    {
        await _authService.Logout();
        return Ok(new { success = true });
    }

    [HttpGet("/logout")]
    public async Task<IActionResult> LogoutAndRedirect()
    {
        await _authService.Logout();
        return new RedirectResult("/login");
    }
}
