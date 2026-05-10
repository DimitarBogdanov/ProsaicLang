namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprFuncCall : NodeExpr
{
    public NodeExprFuncCall(NodeExpr func, List<NodeExpr> args)
        : base($"{func.NiceName}({String.Join(", ", args.Select(a => a.NiceName))})")
    {
        Func = func;
        Args = args;
    }

    public NodeExpr Func { get; }
    public List<NodeExpr> Args { get; }
}