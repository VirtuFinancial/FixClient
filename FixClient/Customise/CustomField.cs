/////////////////////////////////////////////////
//
// FIX Client
//
// Copyright @ 2021 VIRTU Financial Inc.
// All rights reserved.
//
// Filename: CustomField.cs
// Author:   Gary Hughes
//
/////////////////////////////////////////////////

namespace FixClient
{
    public class CustomField
    {
        public int Tag
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public static bool operator ==(CustomField lhs, CustomField rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null))
                return true;
            if (ReferenceEquals(null, rhs)) return false;
            if (ReferenceEquals(lhs, null)) return false;
            return lhs.Tag == rhs.Tag;
        }

        public static bool operator !=(CustomField lhs, CustomField rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object rhs)
        {
            var field = rhs as CustomField;
            if (field != null)
            {
                return this == field;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Tag;
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Tag, Name);
        }
    }


}
