using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public abstract class Node
{
    protected Node(string niceName)
    {
        NiceName = niceName;
    }

    public virtual string NiceName { get; }
    
    public required FileLocation Location { get; init; }
    public required List<Token> Tokens { get; init; }

    public abstract void AcceptVisitor(IVisitor visitor);
}