/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Dictionary.VersionFieldCollection.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Fix
{
    public partial class Dictionary
    {
        // TODO -   we need to be able to represent gaps efficiently - it has a minimal effect on size and none on 
        //          performance so perhaps just leave it as is for now.

        [Browsable(false)]
        public abstract class VersionFieldCollection : IEnumerable<Field>
        {
            public virtual Field this[int index]
            {
                get
                {
                    if (index > _fields.Length - 1)
                        return null;
                    return _fields[index];
                }
            }

            public virtual Field this[string tag]
            {
                get 
                {
                    Field field;
                    if (!TryGetValue(tag, out field))
                        throw new ArgumentException(string.Format("Unknown tag {0}", tag));
                    return field;
                }
            }

            public bool TryGetValue(int tag, out Field field)
            {
                if (tag < 1 || tag > _fields.Length)
                {
                    field = null;
                    return false;
                }
                //
                // _fields[0] == Account.Tag == "1"
                //
                field = _fields[tag - 1];
                //
                // We have some place holder values for unused tags so field can be null.
                //
                return true;
            }

            public bool TryGetValue(string tag, out Field field)
            {
                int index;

                if (!int.TryParse(tag, out index))
                {
                    return LookupFieldByName(tag, out field);
                }

                return TryGetValue(index, out field);
            }

            public virtual int Count => _fields.Length;

            public virtual IEnumerator<Field> GetEnumerator()
            {
                foreach (Field field in _fields)
                    yield return field;
            }

            protected bool LookupFieldByName(string tag, out Field field)
            {
                FieldInfo info = GetType().GetField(tag, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

                if(info != null && info.FieldType == typeof(Field))
                {
                    field = (Field)info.GetValue(this);
                    return true;
                }


                field = null;
                return false;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            protected Field[] _fields;
        }
    }
}
