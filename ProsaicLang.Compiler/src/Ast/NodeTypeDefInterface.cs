using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeTypeDefInterface : NodeTypeDef
{
    public NodeTypeDefInterface(string name) : base(name)
    {
    }
    
    public required Token NameToken { get; init; }
    public string Name => NameToken.Value;

    public required List<InterfaceMethod> Methods { get; init; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitTypeDefInterface(this);
    }
}