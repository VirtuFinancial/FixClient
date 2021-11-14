using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Fix.Common;

public sealed class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue> where TKey : IEquatable<TKey>
{
    const int DefaultInitialCapacity = 0;

    static readonly string? KeyTypeName = typeof(TKey).FullName;

    Dictionary<TKey, TValue>? _dictionary;
    List<KeyValuePair<TKey, TValue>>? _list;
    readonly IEqualityComparer<TKey>? _comparer;
    object? _syncRoot;
    readonly int _initialCapacity;

    public OrderedDictionary()
        : this(DefaultInitialCapacity, null)
    {
    }

    public OrderedDictionary(int capacity)
        : this(capacity, null)
    {
    }

    public OrderedDictionary(IEqualityComparer<TKey> comparer)
        : this(DefaultInitialCapacity, comparer)
    {
    }

    public OrderedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
    {
        if (0 > capacity)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity), $"'{nameof(capacity)}' must be non-negative");
        }
        _initialCapacity = capacity;
        _comparer = comparer;
    }

    static TKey ConvertToKeyType(object key)
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (key is TKey k)
        {
            return k;
        }

        throw new ArgumentException($"'{nameof(key)}' must be of type " + KeyTypeName, nameof(key));
    }

    Dictionary<TKey, TValue> Dictionary
    {
        get
        {
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<TKey, TValue>(_initialCapacity, _comparer);
            }
            return _dictionary;
        }
    }

    List<KeyValuePair<TKey, TValue>> List
    {
        get
        {
            if (_list == null)
            {
                _list = new List<KeyValuePair<TKey, TValue>>(_initialCapacity);
            }
            return _list;
        }
    }

    IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return Dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return List.GetEnumerator();
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return List.GetEnumerator();
    }

    public void Insert(int index, TKey key, TValue value)
    {
        if (index > Count || index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        Dictionary.Add(key, value);
        List.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
    }

    void IOrderedDictionary.Insert(int index, object key, object? value)
    {
        throw new NotImplementedException();
    }

    public void RemoveAt(int index)
    {
        if (index >= Count || index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"'{nameof(index)}' must be non-negative and less than the size of the collection");
        }

        TKey key = List[index].Key;

        List.RemoveAt(index);
        Dictionary.Remove(key);
    }

    public void ReplaceKey(TKey existing, TKey replacement)
    {
        if (Dictionary.ContainsKey(replacement))
        {
            throw new ArgumentException($"replacement key {replacement} is already in use", nameof(replacement));
        }

        if (!Dictionary.TryGetValue(existing, out var value))
        {
            throw new ArgumentException($"existing key {existing} can not be found", nameof(existing));
        }

        Dictionary.Remove(existing);
        Dictionary.Add(replacement, value);
    }

    public TValue this[int index]
    {
        get { return List[index].Value; }
        set
        {

            if (index >= Count || index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"'{nameof(index)}' must be non-negative and less than the size of the collection");
            }

            TKey key = List[index].Key;

            List[index] = new KeyValuePair<TKey, TValue>(key, value);
            Dictionary[key] = value;
        }
    }

    public TValue ItemAtIndex(int index)
    {
        return List[index].Value;
    }

    object? IOrderedDictionary.this[int index]
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }

    void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
    {
        Add(key, value);
    }

    public int Add(TKey key, TValue value)
    {
        Dictionary.Add(key, value);
        List.Add(new KeyValuePair<TKey, TValue>(key, value));
        return Count - 1;
    }

    void IDictionary.Add(object key, object? value)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        Dictionary.Clear();
        List.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return Dictionary.ContainsKey(key);
    }

    bool IDictionary.Contains(object key)
    {
        return ContainsKey(ConvertToKeyType(key));
    }

    bool IDictionary.IsFixedSize
    {
        get { return false; }
    }

    public bool IsReadOnly
    {
        get { return false; }
    }

    ICollection IDictionary.Keys
    {
        get { return (ICollection)Keys; }
    }

    public int IndexOfKey(TKey key)
    {
        if (null == key)
        {
            throw new ArgumentNullException(nameof(key));
        }

        for (int index = 0; index < List.Count; index++)
        {
            var entry = List[index];
            var next = entry.Key;
            if (null != _comparer)
            {
                if (_comparer.Equals(next, key))
                {
                    return index;
                }
            }
            else if (key.Equals(next))
            {
                return index;
            }
        }

        return -1;
    }

    public bool Remove(TKey key)
    {
        int index = IndexOfKey(key);

        if (index >= 0)
        {
            if (Dictionary.Remove(key))
            {
                List.RemoveAt(index);
                return true;
            }
        }

        return false;
    }

    void IDictionary.Remove(object key)
    {
        Remove(ConvertToKeyType(key));
    }

    ICollection IDictionary.Values
    {
        get { return (ICollection)Values; }
    }

    public TValue this[TKey key]
    {
        get { return Dictionary[key]; }
        set
        {
            if (Dictionary.ContainsKey(key))
            {
                Dictionary[key] = value;
                List[IndexOfKey(key)] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                Add(key, value);
            }
        }
    }

    object? IDictionary.this[object key]
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }

    void ICollection.CopyTo(Array array, int index)
    {
        ((ICollection)List).CopyTo(array, index);
    }

    public int Count
    {
        get { return List.Count; }
    }

    bool ICollection.IsSynchronized
    {
        get { return false; }
    }

    object ICollection.SyncRoot
    {
        get
        {
            if (_syncRoot == null)
            {
                System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
            }
            return _syncRoot;
        }
    }

    public IEnumerable<TValue> OrderedValues
    {
        get
        {
            if (_list is null)
            {
                throw new Exception("_list is null");
            }
            return _list.Select(item => item.Value);
        }
    }

    public ICollection<TKey> Keys
    {
        get { return Dictionary.Keys; }
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (Dictionary.TryGetValue(key, out var innerValue))
        {
            value = innerValue;
            return true;
        }
        value = default;
        return false;
    }

    public ICollection<TValue> Values
    {
        get { return Dictionary.Values; }
    }

    void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
    {
        Add(item.Key, item.Value);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
    {
        return ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).Contains(item);
    }

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)Dictionary).CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
    {
        return Remove(item.Key);
    }
}

