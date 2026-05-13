using Microsoft.JSInterop;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.WebApp;

public class MyCookieWasm : IMyCookie
{
    readonly IJSRuntime JSRuntime;
    string expires = "";

    public MyCookieWasm(IJSRuntime jsRuntime)
    {
        JSRuntime = jsRuntime;
        ExpireDays = 300;
    }

    public async Task SetCookie(string key, string value, int? days = null)
    {
        var curExp = (days != null) ? (days > 0 ? DateToUTC(days.Value) : "") : expires;
        string cookie = $"{key}={value}; expires={curExp}; path=/;SameSite=Lax";
        await JSRuntime.InvokeVoidAsync("eval", $"document.cookie = \"{cookie}\"");
    }

    public async Task<string> GetCookie(string key, string def = "")
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

    public async Task DeleteCookie(string key)
    {
        await SetCookie(key, string.Empty, -1);
    }

    public int ExpireDays
    {
        set => expires = DateToUTC(value);
    }

    private static string DateToUTC(int days) => DateTime.UtcNow.AddDays(days).ToUniversalTime().ToString("R");
}
