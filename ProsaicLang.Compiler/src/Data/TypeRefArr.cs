using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class TypeRefArr : TypeRef
{
    public TypeRefArr(TypeRef innerType, List<Token> tokens) : base($"{innerType.Name}[]", tokens)
    {
        InnerType = innerType;
    }

    public TypeRef InnerType { get; }
}