using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ESC.CONCOST.Abstract;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequiredGuidAttribute : ValidationAttribute
{
    public override bool IsValid(object value)
    {
        return value switch
        {
            Guid guid => guid != Guid.Empty,
            Guid[] arr => arr.Any(),
            _ => false
        };
    }
}