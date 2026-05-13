using System;
using System.Collections.Generic;
using System.Linq;

namespace ESC.CONCOST.Abstract;

public class OptionTriple<T>
{
    public OptionTriple()
    {
    }

    public OptionTriple(T value, string en, string vi, string ko)
    {
        Value = value;
        TextEn = en;
        TextVi = vi;
        TextKo = ko;
    }

    public T Value { get; set; }
    public string TextEn { get; set; }
    public string TextVi { get; set; }
    public string TextKo { get; set; }
    public dynamic Attributes { get; set; }
}

public class OptionTriples<T> : List<OptionTriple<T>>
{
    private Dictionary<T, int> _dict = new();
    public OptionTriples(params OptionTriple<T>[] items)
    {
        for (int i = 0; i < items.Length; i++)
        {
            this.Add(items[i]);
            _dict.Add(items[i].Value, i);
        }
    }

    public List<OptionTriple<T>> GetList() => this;

    public OptionTriple<T> this[T value]
    {
        get
        {
            if (value != null && _dict.ContainsKey(value))
            {
                return this[_dict[value]];
            }
            else
            {
                return new OptionTriple<T>(default(T), string.Empty, string.Empty, string.Empty);
            }
        }
    }

    public List<OptionTriple<T>> GetFilters(string en, string vi, string ko)
    {
        var list = new List<OptionTriple<T>>();
        list.Add(new(default(T), en, vi, ko));
        list.AddRange(this);
        return list;
    }
}
