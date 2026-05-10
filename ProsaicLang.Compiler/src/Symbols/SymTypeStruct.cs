namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeStruct : SymType
{
    public required string[] FieldNames { get; init; }
    public required SymType[] FieldTypes { get; init; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return FieldTypes.Any(t => t.HasUnresolvedTypeReferences());
    }
}