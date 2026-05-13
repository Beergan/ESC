using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public interface IMyCookie
{
    Task<string> GetCookie(string key, string def = "");

    Task SetCookie(string key, string value, int? days = null);

    Task DeleteCookie(string key);
}