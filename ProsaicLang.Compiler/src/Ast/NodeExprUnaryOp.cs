namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprUnaryOp : NodeExpr
{
    public NodeExprUnaryOp(NodeExpr expr, ExprUnaryOpType op)
        : base($"{op switch {
            ExprUnaryOpType.Not => "!",
            ExprUnaryOpType.Negate => "-",
            ExprUnaryOpType.AbsoluteValue => "+",
            _ => throw new InvalidOperationException() }}{expr.NiceName}")
    {
        Expr = expr;
        Op = op;
    }
    
    public NodeExpr Expr { get; }
    public ExprUnaryOpType Op { get; }
}

public enum ExprUnaryOpType
{
    Not,
    Negate,
    AbsoluteValue
}