namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeStruct : SymType
{
    public SymTypeStruct? ParentStruct { get; set; }
    public required string[] FieldNames { get; init; }
    public required SymType[] FieldTypes { get; init; }
    
    public override bool HasUnresolvedTypeReferences()
    {
        // Use a HashSet to track visited types and prevent infinite loops
        return HasUnresolvedTypeReferences([]);
    }

    private bool HasUnresolvedTypeReferences(HashSet<SymType> visited)
    {
        if (!visited.Add(this)) 
        {
            return false; 
        }

        foreach (var fieldType in FieldTypes)
        {
            SymType current = fieldType;

            while (current is SymTypeAlias alias)
            {
                current = alias.TargetType;
            }

            if (current is SymTypeUnresolved)
            {
                return true;
            }

            if (current is SymTypeStruct nestedStruct)
            {
                if (nestedStruct.HasUnresolvedTypeReferences(visited))
                {
                    return true;
                }
            }
            else
            {
                return current.HasUnresolvedTypeReferences();
            }
        }

        return false;
    }
}