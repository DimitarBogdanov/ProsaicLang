using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprBoolean : NodeExpr
{
    public NodeExprBoolean() : base("boolean")
    {
    }
    
    public required Token ValueToken { get; init; }
    public string ValueStr => ValueToken.Value;
    public bool Value => ValueStr == "true";

    public override string NiceName => IsParenthesized ? $"({ValueStr})" : ValueStr;
}