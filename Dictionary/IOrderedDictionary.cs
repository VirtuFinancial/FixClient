/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: IOrderedDictionary.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using System.Collections.Specialized;

namespace Fix
{
    public partial class Dictionary
    {
        public interface IOrderedDictionary<TKey, TValue> : IOrderedDictionary, IDictionary<TKey, TValue>
        {
            new int Add(TKey key, TValue value);
            void Insert(int index, TKey key, TValue value);
            new TValue this[int index] { get; set; }
        }
    }
}
