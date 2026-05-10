using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprStr : NodeExpr
{
    public NodeExprStr() : base("string")
    {
    }
    
    public required Token ValueToken { get; init; }
    public string Value => ValueToken.Value;

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitExprStr(this);
    }
}