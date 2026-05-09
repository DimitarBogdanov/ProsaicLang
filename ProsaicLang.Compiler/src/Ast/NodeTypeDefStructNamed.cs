using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeTypeDefStructNamed : NodeTypeDefStructAnonymous
{
    public NodeTypeDefStructNamed(string name) : base(name)
    {
    }
    
    public required Token NameToken { get; init; }
    public string Name => NameToken.Value;
}