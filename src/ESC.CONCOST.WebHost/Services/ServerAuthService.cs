using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ESC.CONCOST.Abstract;
using ESC.CONCOST.Base;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ESC.CONCOST.WebHost;

public class ServerAuthService : IAuthService
{
    protected readonly HttpContext _httpCtx;
    protected readonly UserManager<SA_USER> _userMgr;
    protected readonly IServiceProvider _svcProvider;
    protected readonly SignInManager<SA_USER> _signInMgr;
    protected readonly RoleManager<IdentityRole> _roleManager;
    protected readonly IConfiguration _config;
    protected readonly IMyCookie _cookie;
    protected readonly IPermissionVersionService _versionSvc;
    protected IMyContext _ctx;

    public ServerAuthService(
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
        _httpCtx = httpContextAccessor.HttpContext;
        _userMgr = userManager;
        _svcProvider = svcProvider;
        _signInMgr = signInManager;
        _roleManager = roleManager;
        _config = config;
        _cookie = cookie;
        _versionSvc = versionSvc;
        _ctx = ctx;
    }

    private InfoUser _currentUser;

    public InfoUser CurrentUser => _currentUser;

    [AllowAnonymous]
    [HttpGet]
    public bool IsAuthenthenticated()
    {
        return _httpCtx.User?.Identity?.IsAuthenticated ?? false;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<InfoUser> ValidateTokenInCookie()
    {
        var user = _httpCtx?.User;
        ClaimsPrincipal principal = user;

        // If Middleware failed (due to version mismatch), manually read and VALIDATE the cookie
        if (user == null || !user.Identity.IsAuthenticated)
        {
            if (_httpCtx.Request.Cookies.ContainsKey("Auth"))
            {
                var strToken = _httpCtx.Request.Cookies["Auth"];
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _config["JwtToken:Issuer"],
                        ValidAudience = _config["JwtToken:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtToken:SigningKey"])),
                        ClockSkew = TimeSpan.Zero
                    };

                    principal = handler.ValidateToken(strToken, validationParameters, out var _);
                }
                catch
                {
                    // If validation fails (tempered, expired, etc.), principal remains unauthenticated
                }
            }
        }

        string userName = principal?.Identity?.Name ?? string.Empty;
        var userModel = new InfoUser();

        var repoUser = _ctx.Repo<SA_USER>();
        var entityUser = await repoUser.GetOne(x => x.UserName == userName);

        if (entityUser != null)
        {
            userModel.IsAuthenticated = principal?.Identity?.IsAuthenticated ?? false;
            userModel.UserName = entityUser.UserName;
            userModel.FirstName = entityUser.FirstName;
            userModel.LastName = entityUser.LastName;
            userModel.Avatar = entityUser.Avatar ?? "";

            bool isOutdated = false;
            // 1. Check Role Versions
            var roleVersionClaims = principal.Claims.Where(x => x.Type.StartsWith("RoleVersion_")).ToList();
            foreach (var claim in roleVersionClaims)
            {
                var roleId = claim.Type.Replace("RoleVersion_", "");
                int.TryParse(claim.Value, out int tokenRoleVersion);
                if (_versionSvc.GetRoleVersion(roleId) != tokenRoleVersion)
                {
                    isOutdated = true;
                    break;
                }
            }

            // 2. Check User Version
            if (!isOutdated)
            {
                var userVersionClaim = principal.Claims.FirstOrDefault(x => x.Type == "UserVersion")?.Value;
                int.TryParse(userVersionClaim, out int tokenUserVersion);
                if (_versionSvc.GetUserVersion(userName) != tokenUserVersion)
                {
                    isOutdated = true;
                }
            }

            if (isOutdated)
            {
                // FORCE LOGOUT DUE TO VERSION MISMATCH
                await _cookie.DeleteCookie("Auth");
                userModel.IsAuthenticated = false;
            }
            else if (!principal.Identity.IsAuthenticated)
            {
                userModel.IsAuthenticated = false;
            }
            else
            {
                userModel.IsAuthenticated = true;
                userModel.Claims = principal.Claims
                    .Select(x => new InfoUserClaim(x.Type, x.Value))
                    .ToList();
            }
        }

        _currentUser = userModel;
        return userModel;
    }

    private async Task<List<Claim>> BuildUserClaims(SA_USER user)
    {
        List<Claim> listClaims = new()
        {
            new Claim("sub", user.UserName),
            new Claim("jti", Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("GuidEmployee", user.GuidEmployee.ToString()),
            new Claim("FirstName", user.FirstName ?? ""),
            new Claim("LastName", user.LastName ?? ""),
           
        };
       
        if (_userMgr.SupportsUserRole)
        {
            var roles = await _userMgr.GetRolesAsync(user);
            foreach (var roleName in roles)
            {
                listClaims.Add(new Claim("RoleClaimType", roleName));
                if (_roleManager.SupportsRoleClaims)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        listClaims.Add(new Claim("RoleId", role.Id));
                        listClaims.Add(new Claim($"RoleVersion_{role.Id}", _versionSvc.GetRoleVersion(role.Id).ToString()));

                        var tempList = await _roleManager.GetClaimsAsync(role);
                        foreach (var claimNew in tempList)
                        {
                            var claimOld = listClaims.FirstOrDefault(x => x.Type == claimNew.Type);
                            if (claimOld is null)
                            {
                                listClaims.Add(claimNew);
                            }
                            else
                            {
                                if (int.TryParse(claimOld.Value, out int valueOld))
                                {
                                    int valueNew = int.Parse(claimNew.Value);
                                    var claimCombine = new Claim(claimOld.Type, (valueNew | valueOld).ToString(), claimOld.ValueType, claimOld.Issuer);
                                    listClaims.Remove(claimOld);
                                    listClaims.Add(claimCombine);
                                }
                                else
                                {
                                    listClaims.Add(claimNew);
                                }
                            }
                        }
                    }
                }
            }
        }

        listClaims.Add(new Claim("UserVersion", _versionSvc.GetUserVersion(user.UserName).ToString()));
        return listClaims;
    }

    [HttpPost]
    public async Task<RspLogin> Login(LoginRequest request)
    {
        try
        {
            var user = await _userMgr.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return new RspLogin("WRONG_USER_OR_PWD");
            }
            var singInResult = await _signInMgr.CheckPasswordSignInAsync(user, request.Password, false);
            if (!singInResult.Succeeded)
            {
                return new RspLogin("WRONG_USER_OR_PWD");
            }

            var entityUser = await _userMgr.FindByNameAsync(request.UserName);
            var listClaims = await BuildUserClaims(entityUser);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("JwtToken:SigningKey")));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var timeoutMinutes = _config.GetValue<int>("JwtToken:TokenTimeoutMinutes");
            if (timeoutMinutes <= 0) timeoutMinutes = 60;
            var expires = DateTime.UtcNow.AddMinutes(timeoutMinutes);

            var token = new JwtSecurityToken(
              _config.GetValue<string>("JwtToken:Issuer"),
              _config.GetValue<string>("JwtToken:Audience"),
              listClaims.ToArray(),
              expires: expires,
              signingCredentials: credentials);

            var strToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Set cookie for the same duration (converting minutes to days)
            await _cookie.SetCookie("Auth", strToken, (int)Math.Ceiling(timeoutMinutes / 1440.0));
            var rspLogin = new RspLogin(strToken, expires, entityUser.FirstName, entityUser.LastName, entityUser.Avatar);
            if (rspLogin.Success)
            {
                //var _appManifest = GlobalSetingCommon._AppManifest.FirstOrDefault(x => x.EnterpriseCode == request.EnterpriseCode);
                //rspLogin.AppManifest = _appManifest ?? new();
            }
            return rspLogin;
        }
        catch (Exception ex)
        {
            return new RspLogin(ex.Message);
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<Tuple<bool, string>> Logout()
    {
        try
        {
            if (_httpCtx.User.Identity.IsAuthenticated)
            {
                await _cookie.DeleteCookie("Auth");
            }

            return new Tuple<bool, string>(true, string.Empty);
        }
        catch (Exception ex)
        {
            return new Tuple<bool, string>(false, ex.Message);
        }
    }
}