
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public interface ITextTranslator
{
    string this[string ko, string en]
    {
        get;
    }

    string this[string str]
    {
        get;
    }

    string CurrentLang {get;}
    bool IsKorean => CurrentLang == "ko";

    void SetLang(string lang);
}