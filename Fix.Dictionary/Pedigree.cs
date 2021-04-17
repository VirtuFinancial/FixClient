namespace Fix
{
    public static partial class Dictionary
    {

        public record Pedigree
        {
            public string? Added { get; init; }
            public string? AddedEP { get; init; }
            public string? Updated { get; init; }
            public string? UpdatedEP { get; init; }
            public string? Deprecated { get; init; }
            public string? DeprecatedEP { get; init; }
        }
    }
}