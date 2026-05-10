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
    
    public void AddSymbol(Sym sym)
    {
        sym.SetDefinedInTable(this);
        _symbols.Add(sym);
    }
}