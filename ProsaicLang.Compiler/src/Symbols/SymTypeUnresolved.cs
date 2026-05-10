using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeUnresolved : SymType
{
    private SymTypeUnresolved()
    {
    }

    /// <summary>
    /// This method parses brackets ([]) and returns nested SymTypeArrays if necessary.
    /// </summary>
    public static SymType CreateUnresolvedByName(string name, FileLocation location)
    {
        int depth = 0;
        while (name.EndsWith("[]"))
        {
            depth++;
            name = name[..^2];
        }

        SymType root = new SymTypeUnresolved
        {
            Name = name,
            Location = location
        };

        for (int i = 0; i < depth; i++)
        {
            root = new SymTypeArray
            {
                ElementType = root,
                Name = $"{root.Name}[]",
                Location = location
            };
        }

        return root;
    }
    
    public override bool HasUnresolvedTypeReferences()
    {
        return true;
    }
}