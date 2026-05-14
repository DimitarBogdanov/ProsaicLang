using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.Symbols;

public abstract class Sym
{
    public ScopedSymbolTable? DefinedInTable { get; private set; }
    
    public required string Name { get; set; }
    public required FileLocation Location { get; init; }

    public abstract bool HasUnresolvedTypeReferences();

    public void SetDefinedInTable(ScopedSymbolTable table)
    {
        if (DefinedInTable != null)
        {
            throw new InvalidOperationException("Symbol already defined in a table");
        }
        
        DefinedInTable = table;
    }
}