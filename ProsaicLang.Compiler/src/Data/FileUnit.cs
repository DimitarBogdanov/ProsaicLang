using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.Data;

public sealed class FileUnit
{
    public required string FileName { get; init; }
    public required ModuleNameRef? ModuleNameRef { get; init; }
    public required List<ModuleNameRef> Imports { get; init; }
    public required List<NodeFuncDef> FuncDefs { get; init; }
    public required List<NodeTypeDef> TypeDefs { get; init; }
    public required ScopedSymbolTable SymbolTable { get; init; }
}