namespace ProsaicLang.Compiler.Ast;

public sealed class NodeStatFuncCall : NodeStat
{
    public NodeStatFuncCall(NodeExprFuncCall funcCallExpr) : base($"{funcCallExpr.NiceName};")
    {
        FuncCallExpr = funcCallExpr;
    }
    
    public NodeExprFuncCall FuncCallExpr { get; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitStatFuncCall(this);
    }
}