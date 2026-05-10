using ProsaicLang.Compiler.Scanning;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.Data;

public sealed class StructFields
{
    public required FileLocation Location { get; init; }
    public required List<Token> Tokens { get; init; }
    
    public required string[] Names { get; init; }
    public required TypeRef[] Types { get; init; }
    
    /// <summary>
    /// Null for field indexes which are not anonymous structs.
    /// </summary>
    public required SymTypeStruct?[] InnerAnonymousStructs { get; init; }
}