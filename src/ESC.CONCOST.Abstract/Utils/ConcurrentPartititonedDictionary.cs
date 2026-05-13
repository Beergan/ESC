using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ESC.CONCOST.Abstract;

public enum SerializationType
{
    Xml,
    Json
}

public class ConcurrentPartititonedDictionary<TKey, TValue>
{
    private Dictionary<TKey, TValue>[] _internalDict;
    private int _totalPartition;
    //private readonly IEqualityComparer<TKey> _keyEqualityComparer = null;

    public ConcurrentPartititonedDictionary(int totalParititonRequired = 2,
        int initialCapacity = 0,
        IEqualityComparer<TKey> keyEqualityComparer = null)
    {
        _totalPartition = Math.Max(2, totalParititonRequired);
        _internalDict = new Dictionary<TKey, TValue>[_totalPartition];
        initialCapacity = Math.Max(0, initialCapacity);
        if (initialCapacity != 0)
        {
            int initCapa = (int)Math.Ceiling(((double)initialCapacity) / _totalPartition);
            Parallel.For(0, _totalPartition, i =>
            {
                _internalDict[i] = new Dictionary<TKey, TValue>(initCapa, keyEqualityComparer);
            });
        }
        else
        {
            Parallel.For(0, _totalPartition, i =>
            {
                _internalDict[i] = new Dictionary<TKey, TValue>(keyEqualityComparer);
            });
        }
    }

    public bool TryAdd(TKey key, TValue value)
    {
        var outDict = _internalDict[(((uint)key.GetHashCode()) % _totalPartition)];
        Monitor.Enter(outDict);
        if (outDict.ContainsKey(key))
        {
            Monitor.Exit(outDict);
            return false;
        }
        else
        {
            outDict[key] = value;
            Monitor.Exit(outDict);
            return true;
        }
    }

    public bool TryRemove(TKey key, out TValue value)
    {
        var outDict = _internalDict[(((uint)key.GetHashCode()) % _totalPartition)];
        Monitor.Enter(outDict);
        if (outDict.TryGetValue(key, out value))
        {
            outDict.Remove(key);
            Monitor.Exit(outDict);
            return true;
        }
        Monitor.Exit(outDict);
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _internalDict[(((uint)key.GetHashCode()) % _totalPartition)].TryGetValue(key, out value);
    }

    public TValue GetOrAdd(TKey key, TValue value)
    {
        var outDict = _internalDict[(((uint)key.GetHashCode()) % _totalPartition)];
        Monitor.Enter(outDict);
        if (outDict.TryGetValue(key, out value))
        {
            Monitor.Exit(outDict);
            return value;
        }
        else
        {
            outDict[key] = value;
            Monitor.Exit(outDict);
            return value;
        }
    }

    public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
    {
        var outDict = _internalDict[(((uint)key.GetHashCode()) % _totalPartition)];
        TValue value;
        Monitor.Enter(outDict);
        if (outDict.TryGetValue(key, out value))
        {
            if (value.Equals(comparisonValue))
                outDict[key] = newValue;
            Monitor.Exit(outDict);
            return true;
        }
        else
        {
            Monitor.Exit(outDict);
            return false;
        }
    }

    public TValue this[TKey key]
    {
        get
        {
            return _internalDict[(((uint)key.GetHashCode()) % _totalPartition)][key];
        }
        set
        {
            var outDict = _internalDict[(((uint)key.GetHashCode()) % _totalPartition)];
            Monitor.Enter(outDict);
            outDict[key] = value;
            Monitor.Exit(outDict);
        }
    }

    public bool ContainsKey(TKey key)
    {
        return _internalDict[(((uint)key.GetHashCode()) % _totalPartition)].ContainsKey(key);
    }

    public void Clear()
    {
        Parallel.For(0, _totalPartition, i =>
        {
            var outDict = _internalDict[i];
            Monitor.Enter(outDict);
            outDict.Clear();
            Monitor.Exit(outDict);
        });
    }

    public long Count
    {
        get
        {
            long totCount = 0;
            Parallel.For(0, _totalPartition, () => 0, (int i, ParallelLoopState s, long pCount) =>
            {
                var outDict = _internalDict[i];
                Monitor.Enter(outDict);
                pCount += (long)outDict.Count;
                Monitor.Exit(outDict);
                return pCount;
            }, (long pCount) => Interlocked.Add(ref totCount, pCount));
            return totCount;
        }
    }

    public IEnumerable<TKey> Keys
    {
        get
        {
            for (int i = 0; i < _totalPartition; i++)
            {
                var outDict = _internalDict[i];
                Monitor.Enter(outDict);
                var kvpColl = outDict.Keys;
                Monitor.Exit(outDict);
                foreach (var cKvp in kvpColl)
                    yield return cKvp;
            }
        }
    }

    public IEnumerable<TValue> Values
    {
        get
        {
            for (int i = 0; i < _totalPartition; i++)
            {
                var outDict = _internalDict[i];
                Monitor.Enter(outDict);
                var kvpColl = outDict.Values;
                Monitor.Exit(outDict);
                foreach (var cKvp in kvpColl)
                    yield return cKvp;
            }
        }
    }

    public bool TryAdd(KeyValuePair<TKey, TValue> item)
    {
        return TryAdd(item.Key, item.Value);
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> GetEnumerable()
    {
        for (int i = 0; i < _totalPartition; i++)
        {
            var outDict = _internalDict[i];
            Monitor.Enter(outDict);
            var kvpColl = outDict.ToList();
            Monitor.Exit(outDict);
            foreach (var cKvp in kvpColl)
                yield return cKvp;
        }
    }

    public void Serialize(FileInfo serialFile, SerializationType serialType = SerializationType.Json)
    {
        serialFile.Refresh();
        if (!serialFile.Exists)
        {
            serialFile.Directory.Create();
        }

        for(int i = 0; i< _totalPartition; i++)
        {
            Monitor.Enter(_internalDict[i]);
        }

        if (serialType == SerializationType.Xml)
        {
            using (var writer = new FileStream(serialFile.FullName, FileMode.Create, FileAccess.Write))
            {
                var ser = new DataContractSerializer(_internalDict.GetType());
                ser.WriteObject(writer, _internalDict);
            }
        }
        else
        {
            using (var writer = new FileStream(serialFile.FullName, FileMode.Create, FileAccess.Write))
            {
                var ser = new DataContractJsonSerializer(_internalDict.GetType());
                ser.WriteObject(writer, _internalDict);
            }
        }

        for(int i = 0; i< _totalPartition; i++)
        {
            Monitor.Exit(_internalDict[i]);
        }
    }

    public void DeserializeAndMerge(FileInfo serialFile, SerializationType serialType = SerializationType.Json)
    {
        serialFile.Refresh();
        if (!serialFile.Exists)
        {
            throw new FileNotFoundException("Given serialized file doesn't exist!");
        }

        Dictionary<TKey, TValue>[] deserialDict;
        if (serialType == SerializationType.Xml)
        {
            using (var reader = new FileStream(serialFile.FullName, FileMode.Open, FileAccess.Read))
            {
                var ser = new DataContractSerializer(_internalDict.GetType());
                deserialDict = (Dictionary<TKey, TValue>[])ser.ReadObject(reader);
            }
        }
        else
        {
            using (var reader = new FileStream(serialFile.FullName, FileMode.Open, FileAccess.Read))
            {
                var ser = new DataContractJsonSerializer(_internalDict.GetType());
                deserialDict = (Dictionary<TKey, TValue>[])ser.ReadObject(reader);
            }
        }

        Parallel.For(0, deserialDict.Length, index =>
        {
            Parallel.ForEach(deserialDict[index], kvp => TryAdd(kvp));
        });
    }
}