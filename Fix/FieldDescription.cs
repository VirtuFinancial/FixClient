namespace Fix
{
    public record FieldDescription(int Tag, 
                                   string Value, 
                                   string Name, 
                                   string Description, 
                                   bool Required, 
                                   int Depth,
                                   string DataType,
                                   Dictionary.Pedigree Pedigree,
                                   Dictionary.FieldValue? ValueDefinition);
}