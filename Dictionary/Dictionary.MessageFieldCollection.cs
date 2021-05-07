/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Dictionary.MessageFieldCollection.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System.Collections.Generic;
using System.ComponentModel;

namespace Fix
{
    public partial class Dictionary
    {
        [Browsable(false)]
        public class MessageFieldCollection : IEnumerable<Field>
        {
            protected MessageFieldCollection(IEnumerable<Field> fields)
            {
                foreach (var field in fields)
                {
                    _fields.Add(field.Tag, field);
                }
            }

            public bool TryGetValue(int tag, out Field result)
            {
                return _fields.TryGetValue(tag, out result);
            }

            public virtual Field this[int index] => _fields.ItemAtIndex(index);

            public virtual int Count => _fields.Count;

            public virtual IEnumerator<Field> GetEnumerator() => _fields.OrderedValues.GetEnumerator();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            readonly OrderedDictionary<int, Field> _fields = new();

        }
    }
}
