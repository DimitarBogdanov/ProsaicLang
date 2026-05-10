using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.Ast;

public class NodeTypeDefStructAnonymous : NodeTypeDef
{
    public NodeTypeDefStructAnonymous() : base("struct")
    {
    }

    protected NodeTypeDefStructAnonymous(string overrideName) : base(overrideName)
    {
    }
    
    public required StructFields Fields { get; init; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitTypeDefStructAnonymous(this);
    }
}