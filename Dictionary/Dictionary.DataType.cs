/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Dictionary.DataType.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Fix
{
    public partial class Dictionary
    {
        [Browsable(false)]
        public class DataTypeCollection : IEnumerable<string>
        {
            public virtual string this[int index] => _dataTypes[index];

            public virtual int Count => _dataTypes.Length;

            public virtual IEnumerator<string> GetEnumerator()
            {
                foreach (string type in _dataTypes)
                    yield return type;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            protected string[] _dataTypes;
        }
    }
}
