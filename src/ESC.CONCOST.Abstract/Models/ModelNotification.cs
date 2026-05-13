using System;

namespace ESC.CONCOST.Abstract;

public class ModelNotification
{
    public DateTime NotityTime { get; set; }

    public string Message { get; set; }

    public string Href { get; set; }

    public DateTime Expired { get; set; }

    public Guid? TargetUserGuid { get; set; }
}