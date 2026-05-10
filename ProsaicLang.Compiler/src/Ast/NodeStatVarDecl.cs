using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeStatVarDecl : NodeStat
{
    public NodeStatVarDecl() : base("var declaration")
    {
    }


    public required Token NameToken { get; init; }
    public required TypeRef? SpecifiedType { get; init; }
    public required NodeExpr? Initializer { get; init; }

    public string Name => NameToken.Value;

    public override string NiceName => Initializer != null
        ? $"var {Name}{(SpecifiedType != null ? $": {SpecifiedType.Name}" : "")} = {Initializer.NiceName};"
        : Name;
}