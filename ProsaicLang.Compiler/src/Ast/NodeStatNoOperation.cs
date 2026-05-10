namespace ProsaicLang.Compiler.Ast;

public sealed class NodeStatNoOperation : NodeStat
{
    public NodeStatNoOperation() : base(";")
    {
    }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitStatNoOperation();
    }
}