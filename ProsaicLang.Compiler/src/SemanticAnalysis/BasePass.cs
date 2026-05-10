using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.SemanticAnalysis;

public abstract class BasePass
{
    protected BasePass(FileUnit file, SemanticAnalyser semanticAnalyser)
    {
        File = file;
        Analyser = semanticAnalyser;
        _symbolTables = [ file.SymbolTable ];
    }

    public FileUnit File { get; }
    protected SemanticAnalyser Analyser { get; }

    private readonly List<ScopedSymbolTable> _symbolTables;

    protected void PushSymbolTable(ScopedSymbolTable st)
    {
        _symbolTables.Add(st);
    }
    
    protected void PopSymbolTable()
    {
        _symbolTables.RemoveAt(_symbolTables.Count - 1);
    }

    protected SymType? FindTypeSymbol(string name)
    {
        int arrayDepth = 0;
        string elementTypeName = name;
        while (elementTypeName.EndsWith("[]"))
        {
            arrayDepth++;
            elementTypeName = elementTypeName[..^2];
        }

        for (int idx = _symbolTables.Count - 1; idx >= 0; idx--)
        {
            var st = _symbolTables[idx];
            Sym? symbol = st.FindSymbol(elementTypeName);
            if (symbol == null)
            {
                continue;
            }
            
            if (symbol is not SymType type)
            {
                Analyser.Messages.Add(new CompilerMessage(
                    CompilerMessageType.Error,
                    $"Referenced symbol '{symbol.Name}' is not a type",
                    symbol.Location
                ));
                return null;
            }

            return type;
        }

        return null;
    }
}