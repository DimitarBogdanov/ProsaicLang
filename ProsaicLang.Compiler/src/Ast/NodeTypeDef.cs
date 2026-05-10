using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.Ast;

public abstract class NodeTypeDef : Node
{
    protected NodeTypeDef(string typeCategoryName) : base(typeCategoryName)
    {
    }
    
    public required Sym Symbol { get; init; }
}