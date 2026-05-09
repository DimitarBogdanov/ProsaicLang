namespace ProsaicLang.Compiler.Ast;

public abstract class NodeStat : Node
{
    protected NodeStat(string statementTypeNiceName) : base(statementTypeNiceName)
    {
    }
}