using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class InterfaceMethod
{
    public required FileLocation Location { get; init; }
    public required List<Token> Tokens { get; init; }
    
    public required Token NameToken { get; init; }
    public required string Name { get; init; }
    public required TypeRef ReturnType { get; init; }
    public required FuncParams Params { get; init; }
}