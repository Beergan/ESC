using System;

namespace ESC.CONCOST.Abstract;

public class BlazorSession
{
    public string UserId { get; set; }

    public string SessionId { get; set; }

    public string ConnType { get;  set; }

    public string IpAddress { get; set; }

    public string Path { get; set; }

    public DateTime Begin { get; set; }
}