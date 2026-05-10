using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeTypeDefAlias : NodeTypeDef
{
    public NodeTypeDefAlias(string name) : base(name)
    {
    }
    
    public required TypeRef TargetType { get; init; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitTypeDefAlias(this);
    }
}