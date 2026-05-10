namespace ProsaicLang.Compiler.Ast;

public abstract class NodeExpr : Node
{
    protected NodeExpr(string exprKindNiceName) : base(exprKindNiceName)
    {
    }
    
    public bool IsParenthesized { get; set; }

    public override string NiceName => IsParenthesized ? $"({base.NiceName})" : base.NiceName;
}