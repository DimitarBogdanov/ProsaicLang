namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprAssignment : NodeExpr
{
    public NodeExprAssignment(NodeExpr target, NodeExpr value)
        : base($"{target.NiceName} = {value.NiceName}")
    {
        Target = target;
        Value = value;
    }
    
    public NodeExpr Target { get; }
    public NodeExpr Value { get; }
}