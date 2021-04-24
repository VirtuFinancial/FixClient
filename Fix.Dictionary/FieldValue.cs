using System;

namespace Fix
{
    public partial class Dictionary
    {
        public sealed record FieldValue(int Tag, string Name, string Value)
        {
            public override int GetHashCode()
            {
                return HashCode.Combine(Tag, Value);
            }

            public bool Equals(FieldValue? other)
            {
                if (other is FieldValue right)
                {
                    return Tag == right.Tag && Value == right.Value;
                }

                return false;
            }
        }
    }
}