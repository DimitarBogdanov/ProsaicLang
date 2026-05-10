namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeAlias : SymType
{
    public required SymType TargetType { get; set; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return TargetType.HasUnresolvedTypeReferences();
    }
}