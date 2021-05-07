/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: OrderedDictionary.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Fix
{
    public partial class Dictionary
    {
        public sealed class OrderedDictionary<TKey, TValue> : IOrderedDictionary<TKey, TValue>
        {
            const int DefaultInitialCapacity = 0;

            static readonly string KeyTypeName = typeof(TKey).FullName;
            static readonly string ValueTypeName = typeof(TValue).FullName;

            static readonly bool ValueTypeIsReferenceType = !typeof(ValueType).IsAssignableFrom(typeof(TValue));

            Dictionary<TKey, TValue> _dictionary;
            List<KeyValuePair<TKey, TValue>> _list;
            readonly IEqualityComparer<TKey> _comparer;
            object _syncRoot;
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

            public OrderedDictionary(int capacity, IEqualityComparer<TKey> comparer)
            {
                if (0 > capacity)
                    throw new ArgumentOutOfRangeException(nameof(capacity), $"'{nameof(capacity)}' must be non-negative");

                _initialCapacity = capacity;
                _comparer = comparer;
            }

            static TKey ConvertToKeyType(object key)
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (key is TKey result)
                    return result;

                throw new ArgumentException($"'{nameof(key)}' must be of type " + KeyTypeName, nameof(key));
            }

            static TValue ConvertToValueType(object value)
            {
                if (value == null)
                {
                    if (ValueTypeIsReferenceType)
                        return default;

                    throw new ArgumentNullException(nameof(value));
                }

                if (value is TValue result)
                    return result;

                throw new ArgumentException($"'{nameof(value)}' must be of type " + ValueTypeName, nameof(value));
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
                    throw new ArgumentOutOfRangeException(nameof(index));

                Dictionary.Add(key, value);
                List.Insert(index, new KeyValuePair<TKey, TValue>(key, value));
            }

            void IOrderedDictionary.Insert(int index, object key, object value)
            {
                Insert(index, ConvertToKeyType(key), ConvertToValueType(value));
            }

            public void RemoveAt(int index)
            {
                if (index >= Count || index < 0)
                    throw new ArgumentOutOfRangeException(nameof(index), $"'{nameof(index)}' must be non-negative and less than the size of the collection");

                TKey key = List[index].Key;

                List.RemoveAt(index);
                Dictionary.Remove(key);
            }

            public void ReplaceKey(TKey existing, TKey replacement)
            {
                if (Dictionary.ContainsKey(replacement))
                    throw new ArgumentException($"replacement key {replacement} is already in use", nameof(replacement));

                if (!Dictionary.TryGetValue(existing, out var value))
                    throw new ArgumentException($"existing key {existing} can not be found", nameof(existing));

                Dictionary.Remove(existing);
                Dictionary.Add(replacement, value);
            }

            public TValue this[int index]
            {
                get { return List[index].Value; }

                set
                {
                    if (index >= Count || index < 0)
                        throw new ArgumentOutOfRangeException(nameof(index), $"'{nameof(index)}' must be non-negative and less than the size of the collection");

                    TKey key = List[index].Key;

                    List[index] = new KeyValuePair<TKey, TValue>(key, value);
                    Dictionary[key] = value;
                }
            }

            public TValue ItemAtIndex(int index)
            {
                return List[index].Value;
            }

            object IOrderedDictionary.this[int index]
            {
                get { return this[index]; }

                set { this[index] = ConvertToValueType(value); }
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

            void IDictionary.Add(object key, object value)
            {
                Add(ConvertToKeyType(key), ConvertToValueType(value));
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
                    throw new ArgumentNullException(nameof(key));

                for (int index = 0; index < List.Count; index++)
                {
                    KeyValuePair<TKey, TValue> entry = List[index];
                    TKey next = entry.Key;
                    if (null != _comparer)
                    {
                        if (_comparer.Equals(next, key))
                        {
                            return index;
                        }
                    }
                    else if (next.Equals(key))
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

            object IDictionary.this[object key]
            {
                get { return this[ConvertToKeyType(key)]; }
                set { this[ConvertToKeyType(key)] = ConvertToValueType(value); }
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
                get { return _list.Select(item => item.Value); }
            }

            public ICollection<TKey> Keys
            {
                get { return Dictionary.Keys; }
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return Dictionary.TryGetValue(key, out value);
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
    }
}
