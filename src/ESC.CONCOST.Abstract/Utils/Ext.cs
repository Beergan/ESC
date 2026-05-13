using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ESC.CONCOST.Abstract;

public static class Ext
{
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
    {
        return source ?? Enumerable.Empty<T>();
    }

    public static object NullIf(this bool? condiction, object @true, object @false, object @null)
    {
        return condiction == null ? @null : (condiction.Value ? @true : @false);
    }

    public static Dictionary<string, object> ToDictionary(dynamic dynObj)
    {
        var dictionary = new Dictionary<string, object>();
        foreach (PropertyDescriptor propertyDescriptor in TypeDescriptor.GetProperties(dynObj))
        {
            object obj = propertyDescriptor.GetValue(dynObj);
            dictionary.Add(propertyDescriptor.Name, obj);
        }
        return dictionary;
    }

    public static List<PropertyInfo> ExtractProperties<T>(Type type)
    {
        PropertyInfo[] props = type.GetProperties();
        List<PropertyInfo> columns = new List<PropertyInfo>();

        foreach (PropertyInfo prop in props)
        {
            if (prop.GetCustomAttributes(false)
                .Where(p => p.GetType() == typeof(T))
                .FirstOrDefault() == null)
            {
                continue;
            }

            columns.Add(prop);
        }

        return columns;
    }
}