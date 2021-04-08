/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: EnumDescription.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace FixClient
{
    public class EnumDescription
    {
        public EnumDescription(string description, int value)
        {
            Description = description;
            Value = value;
        }

        public string Description { get; }
        public int Value { get; }

        public override string ToString()
        {
            return Description;
        }
    }

    public class EnumDescriptionCollection : List<EnumDescription>
    {
        public EnumDescriptionCollection(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException(string.Format("enumType must be an enum! you passed a {0}", enumType));
            }

            foreach (var item in enumType.GetEnumValues())
            {
                Add(new EnumDescription(item.ToString(), (int)item));
            }
        }
    }
}
