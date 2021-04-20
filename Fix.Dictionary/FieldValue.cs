namespace Fix
{
    public partial class Dictionary
    {
        public record FieldValue(int Tag, string Name, string Value);
    }
}