namespace ProsaicLang.Compiler.Ast;

public abstract class NodeTypeDef : Node
{
    protected NodeTypeDef(string typeCategoryName) : base(typeCategoryName)
    {
    }
}