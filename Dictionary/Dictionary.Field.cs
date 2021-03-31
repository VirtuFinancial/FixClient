/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Dictionary.Field.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

﻿using System;
using System.ComponentModel;

namespace Fix
{
    public static partial class Dictionary
    {
        public class Field
        {
            const string Category = "Field";
            readonly FieldDefinition _definition;

            public Field()
            {
            }

            // This constructor is for the version.fields
            public Field(FieldDefinition definition)
            {
                _definition = definition;
            }
            
            // This constructor is for the message fields
            public Field(FieldDefinition definition, bool required, int indent, string added)
            :   this(definition)
            {
                Required = required;
                Indent = indent;
                Added = added;
            }

            [Category(Category)]
            public int Tag => _definition.Tag;

            [Category(Category)] 
            public string Name => _definition.Name;

            [Browsable(false)]
            public string Description => _definition.Description;

            [Category(Category)]
            public string DataType => _definition.DataType;

            [Category(Category)]
            [Browsable(false)]
            public Type EnumeratedType => _definition.EnumeratedType;

            [Category(Category)]
            public bool Required { get; }

            [Category(Category)]
            public int Indent { get; }

            [Category(Category)]
            public string Added { get; }

            #region Object Methods

            public override string ToString() => Name;

            #endregion
        }
    }
}
