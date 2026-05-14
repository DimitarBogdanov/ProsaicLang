using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeTypeDefStruct : NodeTypeDef
{
    public NodeTypeDefStruct(string name) : base(name)
    {
    }
    
    public required Token NameToken { get; init; }
    public string Name => NameToken.Value;
    public required StructFieldData FieldData { get; init; }

    public override void AcceptVisitor(IVisitor visitor)
    {
        visitor.VisitTypeDefStruct(this);
    }
}