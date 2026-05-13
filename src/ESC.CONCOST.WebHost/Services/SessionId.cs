using ESC.CONCOST.Abstract;

namespace ESC.CONCOST.WebHost;

public class SessionId : ISessionId
{
    private Guid _value = Guid.NewGuid();

    public string Value => _value.ToString();
}