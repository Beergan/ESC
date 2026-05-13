using System;
using System.Collections.Generic;
using System.Globalization;

namespace ESC.CONCOST.Abstract;

public static class StringHelper
{
    public static string GetBefore(this string str, string key)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        int idx = str.IndexOf(key);

        if (idx > -1)
            return str.Substring(0, idx);
        else
            return str;
    }

    public static string GetBeforeLast(this string str, string key)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        int idx = str.LastIndexOf(key);

        if (idx > -1)
            return str.Substring(0, idx);
        else
            return str;
    }

    public static string GetAfter(this string str, string key)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        int idx = str.IndexOf(key);

        if (idx > -1)
            return str.Substring(idx + key.Length);
        else
            return str;
    }

    public static string GetAfterLast(this string str, string key)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        int idx = str.LastIndexOf(key);

        if (idx > -1)
            return str.Substring(idx + key.Length);
        else
            return str;
    }

    public static string NullIfWhiteSpace(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) { return null; }
        return value.Trim();
    }

    public static int? NullIfZero(this int value)
    {
        if (value == 0)
            return null;

        return value;
    }

    public static string RootAsEmpty(this string value)
    {
        if (value == "root")
            return string.Empty;

        return value.Trim();
    }

    public static int TryParseInt(this string value)
    {
        int r = 1;
        return int.TryParse(value, out r) ? r : 1;
    }

    public static string GetValue(Dictionary<string, string> values, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (values.TryGetValue(key, out var value))
            {
                return value?.Trim() ?? string.Empty;
            }
        }

        return string.Empty;
    }

    public static decimal GetDecimalValue(Dictionary<string, string> values, params string[] keys)
    {
        var value = GetValue(values, keys);

        value = value
            .Replace(",", "")
            .Replace("%", "")
            .Trim();

        return decimal.TryParse(
            value,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var result
        ) ? result : 0;
    }

    public static long GetLongValue(Dictionary<string, string> values, params string[] keys)
    {
        var value = GetValue(values, keys);

        value = value
            .Replace(",", "")
            .Replace("원", "")
            .Trim();

        return long.TryParse(
            value,
            NumberStyles.Any,
            CultureInfo.InvariantCulture,
            out var result
        ) ? result : 0;
    }

    public static int GetIntValue(Dictionary<string, string> values, params string[] keys)
    {
        var value = GetValue(values, keys);

        value = value
            .Replace(",", "")
            .Replace("일", "")
            .Trim();

        return int.TryParse(value, out var result) ? result : 0;
    }

    public static DateTime? GetDateValue(Dictionary<string, string> values, params string[] keys)
    {
        var value = GetValue(values, keys);

        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var formats = new[]
        {
        "yyyy-MM-dd",
        "yyyy/MM/dd",
        "yyyy.MM.dd",
        "MM/dd/yyyy",
        "dd/MM/yyyy"
    };

        if (DateTime.TryParseExact(
                value,
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var exactDate))
        {
            return exactDate;
        }

        return DateTime.TryParse(value, out var date) ? date : null;
    }
}