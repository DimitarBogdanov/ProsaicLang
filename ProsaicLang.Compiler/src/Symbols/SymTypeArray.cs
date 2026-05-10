namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeArray : SymType
{
    public required SymType ElementType { get; init; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return ElementType.HasUnresolvedTypeReferences();
    }
}