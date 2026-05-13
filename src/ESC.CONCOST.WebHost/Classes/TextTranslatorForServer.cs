using ESC.CONCOST.Abstract;

public class TextTranslatorForServer : ITextTranslator
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMyCookie _cookie;
    private string langId = string.Empty;

    public TextTranslatorForServer(IHttpContextAccessor accessor, IMyCookie cookie)
    {
        _httpContextAccessor = accessor;
        _cookie = cookie;
        IRequestCookieCollection ckCollection = _httpContextAccessor.HttpContext?.Request.Cookies;
        
        string value = null;
        ckCollection?.TryGetValue(nameof(langId), out value);

        langId = value ?? "ko";
    }

    public string this[string str] 
    {
        get{
            if(string.IsNullOrEmpty(str))
                return str;

            string[] parts = str.Split('|');
            
            // Pattern: KO|EN
            if (langId == "en")
                return parts.Length > 1 ? parts[1] : parts[0];

            return parts[0];
        }
    }

    public string this[string ko, string en] 
    {
        get {
            if (langId == "en") return en;
            return ko;
        }
    }

    public string CurrentLang => langId;

    public void SetLang(string lang)
    {
        _cookie.SetCookie(nameof(langId), lang);
        langId = lang;
    } 
}