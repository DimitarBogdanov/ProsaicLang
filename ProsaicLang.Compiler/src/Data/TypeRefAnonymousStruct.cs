using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class TypeRefAnonymousStruct : TypeRef
{
    public TypeRefAnonymousStruct(NodeTypeDefStructAnonymous structDef, List<Token> tokens)
        : base("struct", tokens)
    {
        StructDef = structDef;
    }

    public NodeTypeDefStructAnonymous StructDef { get; }
}