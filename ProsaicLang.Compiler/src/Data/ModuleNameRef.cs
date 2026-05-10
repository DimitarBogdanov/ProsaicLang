using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class ModuleNameRef
{
    public required string Name { get; init; }
    public required FileLocation Location { get; init; }
    public required List<Token> Tokens { get; init; }
}