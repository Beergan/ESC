using System.Linq;

namespace ESC.CONCOST.Base;

public class MediatorSerializedObject
{
    public string FullTypeName { get; private set; }

    public string Data { get; private set; }

    public string AdditionalDescription { get; private set; }

    public MediatorSerializedObject(string fullTypeName, string data, string additionalDescription)
    {
        this.FullTypeName = fullTypeName;
        this.Data = data;
        this.AdditionalDescription = additionalDescription;
    }

    /// <summary>
    /// Override for Hangfire dashboard display.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var commandName = this.FullTypeName.Split('.').Last();
        return $"{commandName} {this.AdditionalDescription}";
    }
}