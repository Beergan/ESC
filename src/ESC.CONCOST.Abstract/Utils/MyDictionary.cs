using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;

namespace ESC.CONCOST.Abstract;

public class MyDictionary<TKey, TValue>
{
    private Dictionary<TKey, TValue> _dict;
    public MyDictionary()
    {
        _dict = new Dictionary<TKey, TValue>();
    }

    public void Add(TKey key, TValue value)
    {
        _dict.Add(key, value);
    }

    public MyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
    {
        _dict = new Dictionary<TKey, TValue>(pairs);
    }
    
    public MyDictionary(Dictionary<TKey, TValue> dict)
    {
        _dict = new Dictionary<TKey, TValue>(dict);
    }

    public TValue this[TKey key]
    {
        get
        {
            if (_dict.ContainsKey(key))
            {
                return _dict[key];
            }

            var @default = default(TValue);
            _dict[key] = @default;
            return @default;
        }

        set{
            _dict[key] = value;
        }
    }

    public Dictionary<TKey, TValue> Dict => _dict;
}