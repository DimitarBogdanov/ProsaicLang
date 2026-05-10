using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.SemanticAnalysis;

public sealed class SemanticAnalyser
{
    public SemanticAnalyser(FileUnit file)
    {
        _file = file;
    }
    
    public List<CompilerMessage> Messages { get; } = [];

    private readonly FileUnit _file;

    public void Run()
    {
        IVisitor[] passes = [new TypeResolvePass(_file, this)];

        foreach (IVisitor pass in passes)
        {
            _file.TypeDefs.ForEach(def => def.AcceptVisitor(pass));
            _file.FuncDefs.ForEach(def => def.AcceptVisitor(pass));

            if (Messages.Any(x => x.Type == CompilerMessageType.Error))
            {
                return;
            }
            
            if (_file.SymbolTable.HasSymbolsWithUnresolvedTypeReferences())
            {
                throw new InvalidOperationException(
                    "There are unresolved type references in the symbol table which did not yield a compiler error");
            }
        }
    }
}