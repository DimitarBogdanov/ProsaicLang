using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public static class BinOperators
{
    private static readonly Dictionary<TokenType, int> OpPrecedences = new()
    {
        [TokenType.OpSet] = -1,
        
        [TokenType.OpPlus] = 0,
        [TokenType.OpMinus] = 0,
        
        [TokenType.OpMul] = 1,
        [TokenType.OpDiv] = 1,
        [TokenType.OpMod] = 1,
        
        [TokenType.OpPower] = 2,
        
        [TokenType.OpEq] = 3,
        [TokenType.OpNeq] = 3,
        [TokenType.OpGt] = 3,
        [TokenType.OpGte] = 3,
        [TokenType.OpLt] = 3,
        [TokenType.OpLte] = 3,
    };

    public static bool IsBinOp(TokenType tokenType)
    {
        return OpPrecedences.ContainsKey(tokenType);
    }

    public static int GetPrecedence(TokenType tokenType)
    {
        OpPrecedences.TryGetValue(tokenType, out int precedence);
        return precedence;
    }

    public static bool IsRightAssoc(TokenType tokenType)
    {
        return tokenType == TokenType.OpPower
            || tokenType == TokenType.OpSet;
    }
}