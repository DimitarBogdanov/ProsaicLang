using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public abstract class TypeRef
{
    protected TypeRef(string name, List<Token> tokens)
    {
        Name = name;
        Tokens = tokens;
        Location = FileLocation.CreateRange(tokens.First().Location, tokens.Last().Location);
    }

    public string Name { get; }
    public List<Token> Tokens { get; }
    
    public FileLocation Location { get; }
}