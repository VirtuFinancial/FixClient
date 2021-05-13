namespace Fix
{
    public record FieldDescription(int Tag, 
                                   string Value, 
                                   string Name, 
                                   string Description, 
                                   bool Required, 
                                   int Indent);
}