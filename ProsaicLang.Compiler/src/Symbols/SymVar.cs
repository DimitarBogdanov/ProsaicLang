namespace ProsaicLang.Compiler.Symbols;

public sealed class SymVar : Sym
{
    public required SymType Type { get; init; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return Type.HasUnresolvedTypeReferences();
    }
}