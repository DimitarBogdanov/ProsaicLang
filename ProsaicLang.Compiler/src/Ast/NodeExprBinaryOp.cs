namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprBinaryOp : NodeExpr
{
    public NodeExprBinaryOp(ExprBinaryOpType type, NodeExpr left, NodeExpr right)
        : base($"{left.NiceName} {GetBinaryOperatorSymbol(type)} {right.NiceName}")
    {
        Type = type;
        Left = left;
        Right = right;
    }

    public ExprBinaryOpType Type { get; }
    public NodeExpr Left { get; }
    public NodeExpr Right { get; }

    private static string GetBinaryOperatorSymbol(ExprBinaryOpType type)
    {
        return type switch
        {
            ExprBinaryOpType.Add => "+",
            ExprBinaryOpType.Subtract => "-",
            ExprBinaryOpType.Multiply => "*",
            ExprBinaryOpType.Divide => "/",
            ExprBinaryOpType.Modulo => "%",
            ExprBinaryOpType.Power => "^",
            ExprBinaryOpType.Equal => "==",
            ExprBinaryOpType.NotEqual => "!=",
            ExprBinaryOpType.LessThan => "<",
            ExprBinaryOpType.LessThanOrEqual => "<=",
            ExprBinaryOpType.GreaterThan => ">",
            ExprBinaryOpType.GreaterThanOrEqual => ">=",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitExprBinaryOp(this);
    }
}

public enum ExprBinaryOpType
{
    Add,
    Subtract,
    Multiply,
    Divide,
    Modulo,
    Power,
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
}