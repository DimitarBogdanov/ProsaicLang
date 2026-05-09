namespace ProsaicLang.Compiler.Ast;

public abstract class NodeExpr : Node
{
    protected NodeExpr(string exprKindNiceName) : base(exprKindNiceName)
    {
    }
}