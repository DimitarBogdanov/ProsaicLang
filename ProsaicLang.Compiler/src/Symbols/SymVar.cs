namespace ProsaicLang.Compiler.Symbols;

public sealed class SymVar : Sym
{
    public required SymType Type { get; set; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return Type.HasUnresolvedTypeReferences();
    }
}