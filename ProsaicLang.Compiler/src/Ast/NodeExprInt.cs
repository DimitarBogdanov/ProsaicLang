using System.Numerics;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Ast;

public sealed class NodeExprInt : NodeExpr
{
    public NodeExprInt() : base("integer")
    {
    }
    
    public required Token ValueToken { get; init; }
    public string ValueStr => ValueToken.Value;
    public BigInteger ValueBigInt => BigInteger.Parse(ValueStr);
    
    public override string NiceName => IsParenthesized ? $"({ValueStr})" : ValueStr;
}