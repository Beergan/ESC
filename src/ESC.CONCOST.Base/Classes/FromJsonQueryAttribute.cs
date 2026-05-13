using Microsoft.AspNetCore.Mvc;

namespace ESC.CONCOST.Base;

public class FromJsonQueryAttribute : ModelBinderAttribute
{
    public FromJsonQueryAttribute()
    {
        BinderType = typeof(JsonQueryBinder);
    }
}