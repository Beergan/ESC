using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace ESC.CONCOST.Abstract;

public class OptionDual<T>
{
    public OptionDual()
    {
    }

    public OptionDual(T value, string en, string vi)
    {
        Value = value;
        TextEn = en;
        TextVi = vi;
    }

    public T Value { get; set; }

    public string TextEn { get; set; }

    public string TextVi { get; set; }

    public dynamic Attributes { get; set; }
}

public class OptionDuals<T> : List<OptionDual<T>>
{
    private Dictionary<T, int> _dict = new();
    public OptionDuals(params OptionDual<T>[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            this.Add(items[i]);
            _dict.Add(items[i].Value, i);
        }
    }

    public List<OptionDual<T>> GetList() => this;

    public OptionDual<T> this[T value]
    {
        get
        {
            if (value != null && _dict.ContainsKey(value))
            {
                return this[_dict[value]];
            }
            else
            {
                return new OptionDual<T>(default(T), string.Empty, string.Empty);
            }
        }
    }

    public List<OptionDual<T>> GetFilters(string en, string vi)
    {
        var list = new List<OptionDual<T>>();
        list.Add(new(default(T), en, vi));
        list.AddRange(this);
        return list;
    }

    public List<OptionDual<T>> GetFilters(T value, string en, string vi)
    {
        var list = new List<OptionDual<T>>();
        list.Add(new(value, en, vi));
        list.AddRange(this);
        return list;
    }
}

public static class OptionDualUtil
{
    public static List<OptionDual<T>> GetFilters<T>(this List<OptionDual<T>> list, string en, string vi)
    {
        var newList = new List<OptionDual<T>>();
        newList.Add(new(default(T), en, vi));
        newList.AddRange(list);
        return newList;
    }

    public static OptionDuals<T> Excepts<T>(this List<OptionDual<T>> options, params T[] excepts)
    {
        return new OptionDuals<T>(options.Where(x => !excepts.Any(y => y.Equals(x.Value))).ToArray());
    }

    // public static OptionDual<T> Clone<T>(this OptionDual<T> item)
    // {
    //     return new OptionDual<T> { Value = item.Value, TextEn = item.TextEn, TextVi = item.TextVi };
    // }
}