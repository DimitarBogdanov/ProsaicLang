namespace ProsaicLang.Compiler.Symbols;

public sealed class SymTypeStruct : SymType
{
    public SymTypeStruct? ParentStruct { get; set; }
    public required string[] FieldNames { get; init; }
    public required SymType[] FieldTypes { get; init; }
    
    private bool _hasUnresolvedTypeReferencesVisited;
    
    public override bool HasUnresolvedTypeReferences()
    {
        if (_hasUnresolvedTypeReferencesVisited)
            return false;
        _hasUnresolvedTypeReferencesVisited = true;
        
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

            if (current.HasUnresolvedTypeReferences())
            {
                return true;
            }
        }

        return false;
    }
}