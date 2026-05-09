using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Data;

public sealed class CompilerMessage
{
    public CompilerMessage(CompilerMessageType type, string message, FileLocation location, IEnumerable<Token>? affectedTokens = null)
    {
        Type = type;
        Message = message;
        Location = location;
        AffectedTokens = affectedTokens?.ToList() ?? [];
    }

    public CompilerMessageType Type { get; }
    public string Message { get; }
    public FileLocation Location { get; }
    public List<Token> AffectedTokens { get; }
    
    public override string ToString()
    {
        return $"{Type} :: {Location}: {Message}";
    }
}

public enum CompilerMessageType
{
    Error,
    Warning
}