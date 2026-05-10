using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprDecimal : NodeExpr
{
    public NodeExprDecimal() : base("decimal")
    {
        Value = Convert.ToDecimal(ValueStr);
    }
    
    public required Token ValueToken { get; init; }
    public string ValueStr => ValueToken.Value;
    public decimal Value { get; }
    
    public override string NiceName => IsParenthesized ? $"({ValueStr})" : ValueStr;

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitExprDecimal(this);
    }
}