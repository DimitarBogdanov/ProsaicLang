namespace ProsaicLang.Compiler.Ast;

public sealed class NodeStatNoOperation : NodeStat
{
    public NodeStatNoOperation() : base(";")
    {
    }
}