namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeInterface : SymType
{
    public required string[] MethodNames { get; init; }
    public required SymType[] MethodReturnTypes { get; init; }
    public required string[][] MethodParamNames { get; init; }
    public required SymType[][] MethodParamTypes { get; init; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return MethodParamTypes.Any(t => t.Any(tt => tt.HasUnresolvedTypeReferences()))
            || MethodReturnTypes.Any(t => t.HasUnresolvedTypeReferences());
    }
}