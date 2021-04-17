namespace Fix
{
    public partial class Dictionary
    {
        public struct FieldValue
        {
            public FieldValue(int tag, string name, string value)
            {
                Tag = tag;
                Name = name;
                Value = value;
            }

            public int Tag { get; }
            public string Name { get; }
            public string Value { get; }
        }
    }
}