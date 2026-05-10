namespace ProsaicLang.Compiler.Ast;

public sealed class NodeStatAssignment : NodeStat
{
    public NodeStatAssignment(NodeExprAssignment assignExpr) : base($"{assignExpr.NiceName};")
    {
        AssignExpr = assignExpr;
    }

    public NodeExprAssignment AssignExpr { get; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitStatAssignment(this);
    }
}