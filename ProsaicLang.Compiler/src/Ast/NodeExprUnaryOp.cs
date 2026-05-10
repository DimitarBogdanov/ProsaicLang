namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprUnaryOp : NodeExpr
{
    public NodeExprUnaryOp(NodeExpr expr, ExprUnaryOpType op)
        : base($"{GetUnaryOperatorSymbol(op)}{expr.NiceName}")
    {
        Expr = expr;
        Op = op;
    }

    private static string GetUnaryOperatorSymbol(ExprUnaryOpType op)
    {
        return op switch {
            ExprUnaryOpType.Not => "!",
            ExprUnaryOpType.Negate => "-",
            ExprUnaryOpType.AbsoluteValue => "+",
            _ => throw new InvalidOperationException() };
    }

    public NodeExpr Expr { get; }
    public ExprUnaryOpType Op { get; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitExprUnaryOp(this);
    }
}

public enum ExprUnaryOpType
{
    Not,
    Negate,
    AbsoluteValue
}