namespace ProsaicLang.Compiler.Data;

public sealed class FuncParams
{
    public required string[] Names { get; init; }
    public required TypeRef[] Types { get; init; }
}