namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeUnresolved : SymType
{
    public override bool HasUnresolvedTypeReferences()
    {
        return true;
    }
}