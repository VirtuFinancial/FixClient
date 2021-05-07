/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: Dictionary.FieldDefinition.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

using System;

namespace Fix
{
    public partial class Dictionary
    {
        public class FieldDefinition
        {
            public FieldDefinition()
            {
            }

            public FieldDefinition(int tag, string name, string description, string dataType, Type enumeratedType, string added)
            {
                if (enumeratedType != null && !enumeratedType.IsEnum)
                {
                    throw new ArgumentException("enumerated type must be an enum");
                }

                EnumeratedType = enumeratedType;
                Tag = tag;
                Name = name;
                Description = description;
                DataType = dataType.ToUpper() == "STIRNG" ? "string" : dataType;
                Added = added;
            }

            public int Tag { get; }
            public string Name { get; }
            public string Description { get; }
            public string DataType { get; }
            public Type EnumeratedType { get; }

            public string Added { get; }

            #region Object Methods

            public override string ToString() => Name;

            #endregion
        }
    }
}
