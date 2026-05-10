using ProsaicLang.Compiler.Ast;

namespace ProsaicLang.Compiler.Data;

public sealed class FileUnit
{
    public required string FileName { get; init; }
    public required ModuleNameRef? ModuleNameRef { get; init; }
    public required List<ModuleNameRef> Imports { get; init; }
    public required List<NodeFuncDef> FuncDefs { get; init; }
    public required List<NodeTypeDef> TypeDefs { get; init; }
}