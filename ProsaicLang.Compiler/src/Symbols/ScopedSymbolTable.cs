namespace ProsaicLang.Compiler.Symbols;

public sealed class ScopedSymbolTable
{
    public ScopedSymbolTable()
    {
        _children = new List<ScopedSymbolTable>(0);
    }
    
    public ScopedSymbolTable? Parent { get; private set; }
    public IReadOnlyList<ScopedSymbolTable> Children => _children;

    private readonly List<ScopedSymbolTable> _children;
    private readonly List<Sym> _symbols = new();

    public ScopedSymbolTable Derive()
    {
        ScopedSymbolTable st = new();
        st.Parent = this;
        _children.Add(st);
        return st;
    }

    public bool HasSymbol(string name)
    {
        return _symbols.Any(x => x.Name == name);
    }

    public Sym? FindSymbol(string name)
    {
        return _symbols.FirstOrDefault(x => x.Name == name);
    }
    
    public void AddSymbol(Sym sym)
    {
        sym.SetDefinedInTable(this);
        _symbols.Add(sym);
    }
    
    public bool HasSymbolsWithUnresolvedTypeReferences()
    {
        _symbols.Where(x => x.HasUnresolvedTypeReferences()).ToList().ForEach(x => Console.WriteLine(x.Name));
        return _symbols.Any(x => x.HasUnresolvedTypeReferences())
               || _children.Any(x => x.HasSymbolsWithUnresolvedTypeReferences());
    }
}