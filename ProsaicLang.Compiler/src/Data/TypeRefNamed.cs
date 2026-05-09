using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class TypeRefNamed : TypeRef
{
    public TypeRefNamed(string name, List<Token> tokens) : base(name, tokens)
    {
    }
}