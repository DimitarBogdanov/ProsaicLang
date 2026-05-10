namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeAlias : SymType
{
    public required SymType TargetType { get; init; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return TargetType.HasUnresolvedTypeReferences();
    }
}