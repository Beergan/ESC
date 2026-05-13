using System;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public interface IAuthService
{
    Task<RspLogin> Login(LoginRequest request);

    Task<Tuple<bool, string>> Logout();

    Task<InfoUser> ValidateTokenInCookie();

    InfoUser CurrentUser { get; }
}