using ESC.CONCOST.Abstract;

public class TextTranslatorForWasm : ITextTranslator
{
    private readonly IMyCookie _cookie;
    private string langId = string.Empty;

    public TextTranslatorForWasm(IMyCookie cookie, string lang)
    {
        langId = lang ?? "vi";
        _cookie = cookie;
    }

    public string this[string ko, string en]
    {
        get
        {
            if (langId == "en") return en;
            return ko;
        }
    }

    public string this[string str]
    {
        get
        {
            if (string.IsNullOrEmpty(str))
                return str;

            string[] parts = str.Split('|');

            // Pattern: KO|EN
            if (langId == "en")
                return parts.Length > 1 ? parts[1] : parts[0];

            return parts[0];
        }
    }

    public string CurrentLang => langId;

    public void SetLang(string lang)
    {
        _cookie.GetCookie(nameof(langId), lang);
        langId = lang;
    }
}