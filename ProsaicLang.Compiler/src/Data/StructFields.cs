using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class StructFields
{
    public required FileLocation Location { get; init; }
    public required List<Token> Tokens { get; init; }
    
    public required string[] Names { get; init; }
    public required TypeRef[] Types { get; init; }
}