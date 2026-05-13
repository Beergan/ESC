using Microsoft.JSInterop;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.WebHost;

public class MyCookieServer : IMyCookie
{
    protected readonly HttpContext _httpCtx;
    readonly IJSRuntime JSRuntime;
    string expires = "";

    public MyCookieServer(IHttpContextAccessor httpCtxAccessor, IJSRuntime jsRuntime)
    {
        _httpCtx = httpCtxAccessor.HttpContext;
        JSRuntime = jsRuntime;
        ExpireDays = 300;
    }

    public async Task<string> GetCookie(string key, string def = "")
    {
        string value = string.Empty;
        try
        {
            if (_httpCtx.Request.Path.Value.StartsWith("/_blazor"))
            {
                return await GetCookieBlazor(key, def);
            }
            else
            {
                var cookies = _httpCtx.Request.Cookies;
                if (cookies.ContainsKey(key))
                {
                    return cookies[key];
                }
            }
        }
        catch { }

        return def;
    }

    public async Task SetCookie(string key, string value, int? days = null)
    {
        if (_httpCtx.Request.Path.Value.StartsWith("/_blazor"))
        {
            await SetValueBlazor(key, value, days);
        }
        else
        {
            var cookieOpt = new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) };
            _httpCtx.Response.Cookies.Append(key, value, cookieOpt);
        }
    }

    public async Task DeleteCookie(string key)
    {
        if (_httpCtx.Request.Path.Value.StartsWith("/_blazor"))
        {
            await SetValueBlazor(key, string.Empty);
        }
        else
        {
            _httpCtx.Response.Cookies.Delete(key);
        }
    }

    public async Task SetValueBlazor(string key, string value, int? days = null)
    {
        var curExp = (days != null) ? (days > 0 ? DateToUTC(days.Value) : "") : expires;
        var cookie = $"{key}={value}; expires={curExp}; path=/;SameSite=Lax";
        await JSRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{cookie}\"");
    }

    private async Task<string> GetCookieBlazor(string key, string def = "")
    {
        var cValue = await JSRuntime.InvokeAsync<string>("eval", $"document.cookie");
        if (string.IsNullOrEmpty(cValue)) return def;

        var vals = cValue.Split(';');
        foreach (var val in vals)
        {
            var k = val.GetBefore("=").Trim();
            var v = val.GetAfter("=").Trim();

            if (k == key)
                return v;
        }

        return def;
    }

    public int ExpireDays
    {
        set => expires = DateToUTC(value);
    }

    private static string DateToUTC(int days) => DateTime.UtcNow.AddDays(days).ToUniversalTime().ToString("R");
}