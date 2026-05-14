using ProsaicLang.Compiler.Scanning;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.Data;

public sealed class StructFieldData
{
    public required FileLocation Location { get; init; }
    public required List<Token> Tokens { get; init; }
    
    public required string StructName { get; init; }
    public required SymTypeStruct StructSymbol { get; init; }
    
    public required string[] FieldNames { get; init; }
    public required TypeRef[] FieldTypes { get; init; }
}