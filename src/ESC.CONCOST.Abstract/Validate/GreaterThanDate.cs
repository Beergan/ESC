using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ESC.CONCOST.Abstract;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class DateGreaterThanAttribute : ValidationAttribute
{
    public DateGreaterThanAttribute(string dateField){
        DateField = dateField;
    }
    public string DateField {get;}
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {

        DateTime? endDate = (DateTime?)value;
        DateTime? startDate = (DateTime?)validationContext.ObjectType.GetProperty(DateField)
            .GetValue(validationContext.ObjectInstance, null);
        // DateTime? startDate = earlierDateValue != null ? (DateTime?)earlierDateValue : null;

        if (startDate.HasValue && endDate.HasValue && startDate >= endDate)
        {
            return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName });
        }
        return null;
    }
    
}
