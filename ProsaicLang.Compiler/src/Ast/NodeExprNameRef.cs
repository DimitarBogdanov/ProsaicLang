namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprNameRef : NodeExpr
{
    public NodeExprNameRef(NodeExpr? parent, string referencedName) : base(parent != null ? $"{parent.NiceName}.{referencedName}" : referencedName)
    {
        Parent = parent;
        ReferencedName = referencedName;
    }
    
    public NodeExpr? Parent { get; }
    public string ReferencedName { get; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitExprNameRef(this);
    }
}