using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public static class ChangeTracker
{
    public async static Task<string> CompareObjectsToJson<T>(T oldObj, T newObj)
    {
        var changes = new Dictionary<string, PropertyChange>();
        var props = typeof(T).GetProperties();

        foreach (var prop in props)
        {
            var oldVal = prop.GetValue(oldObj)?.ToString() ?? "";
            var newVal = prop.GetValue(newObj)?.ToString() ?? "";

            if (oldVal != newVal)
            {
                changes[prop.Name] = new PropertyChange
                {
                    Old = oldVal,
                    New = newVal
                };
            }
        }

        return  changes.Count > 0
            ? JsonSerializer.Serialize(changes, new JsonSerializerOptions { WriteIndented = true })
            : "{}";
    }
}
public class PropertyChange
{
    public string Old { get; set; } = "";
    public string New { get; set; } = "";
}