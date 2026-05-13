using Microsoft.Extensions.Localization;
using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.WebHost;

public class MyStringLocalizer : IStringLocalizer
{
    private readonly IServiceProvider _serviceProvider;

    public MyStringLocalizer(IServiceCollection sc)
    {
        _serviceProvider = sc.BuildServiceProvider();
    }

    public LocalizedString this[string str]
    {
        get
        {
            if (string.IsNullOrEmpty(str))
                return new LocalizedString(str, str);

            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            IRequestCookieCollection ckCollection = httpContextAccessor.HttpContext?.Request.Cookies;

            string langId = null;
            ckCollection?.TryGetValue(nameof(langId), out langId);

            if (langId == null)
                langId = "vi";

            string en = str.GetBefore("|");
            string vi = str.GetAfter("|");

            if (langId == "en")
                return new LocalizedString(str, en);

            return new LocalizedString(str, vi);
        }
    }

    public LocalizedString this[string str, params object[] arguments]
    {
        get
        {
            if (string.IsNullOrEmpty(str))
                return new LocalizedString(str, str);

            str = string.Format(str, arguments);
            var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();
            IRequestCookieCollection ckCollection = httpContextAccessor.HttpContext?.Request.Cookies;

            string langId = null;
            ckCollection?.TryGetValue(nameof(langId), out langId);

            if (langId == null)
                langId = "vi";

            string en = str.GetBefore("|");
            string vi = str.GetAfter("|");

            if (langId == "en")
                return new LocalizedString(str, en);

            return new LocalizedString(str, vi);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return new List<LocalizedString>();
    }
}