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

namespace FixClient;

public class CustomField
{
    public int Tag { get; set; }

    public string Name { get; set; } = string.Empty;

    public static bool operator ==(CustomField lhs, CustomField rhs)
    {
        if (lhs is null && rhs is null)
        {
            return true;
        }

        if (rhs is null)
        {
            return false;
        }

        if (lhs is null)
        {
            return false;
        }

        return lhs.Tag == rhs.Tag;
    }

    public static bool operator !=(CustomField lhs, CustomField rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object? rhs)
    {
        if (rhs is CustomField field)
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
