using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class TypeRefUnknown : TypeRef
{
    public TypeRefUnknown(List<Token> tokens) : base("<Unknown>", tokens)
    {
    }
}