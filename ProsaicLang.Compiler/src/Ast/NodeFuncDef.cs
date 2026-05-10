using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeFuncDef : Node
{
    public NodeFuncDef()
        : base("function definition")
    {
    }
    
    public required Token NameToken { get; init; }
    public required FuncParams Params { get; init; }
    public required NodeStat Body { get; init; }
    public required TypeRef? ReturnType { get; init; }
    
    
    public string Name => NameToken.Value;
}