namespace ProsaicLang.Compiler.Symbols;

public sealed class SymFunc : Sym
{
    public required SymType? ReturnType { get; set; }
    public required string[] ParamNames { get; init; }
    public required SymType[] ParamTypes { get; init; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return (ReturnType?.HasUnresolvedTypeReferences() ?? false)
            || ParamTypes.Any(t => t.HasUnresolvedTypeReferences());
    }
}