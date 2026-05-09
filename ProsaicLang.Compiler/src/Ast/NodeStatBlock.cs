namespace ProsaicLang.Compiler.Ast;

public sealed class NodeStatBlock : NodeStat
{
    public NodeStatBlock() : base("statement block")
    {
    }
    
    public required List<NodeStat> Children { get; init; }
}