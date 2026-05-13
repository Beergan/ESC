using System;
using System.ComponentModel.DataAnnotations;

namespace ESC.CONCOST.Abstract;

[AttributeUsage(
AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
AllowMultiple = false)]
public class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute() : base() { }
    public override bool IsValid(object value)
    {
        if (value is null)
            return true; // Allows to return a null value
        switch (value)
        {
            case Guid guid:
                return guid != Guid.Empty; //Checks whether the GUID is empty or not and returns false if GUID is empty
            default:
                return true;
        }
    }
}