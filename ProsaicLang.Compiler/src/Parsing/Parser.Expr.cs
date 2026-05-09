using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsExpr()
    {
        return IsExprPrimitive();
    }

    private NodeExpr ParseExpr()
    {
        if (IsExprPrimitive())
            return ParseExprPrimitive();

        throw new InvalidOperationException("Unknown expression to parse");
    }

    private bool IsExprPrimitive()
    {
        return _stream.CurrentIs(TokenType.Int)
            || _stream.CurrentIs(TokenType.Decimal)
            || _stream.CurrentIs(TokenType.String);
    }

    private NodeExpr ParseExprPrimitive()
    {
        Token tok = _stream.Consume();
        if (tok.Type == TokenType.Int)
            return new NodeExprInt
            {
                ValueToken = tok,
                Location = tok.Location,
                Tokens = [tok]
            };

        if (tok.Type == TokenType.Decimal)
            return new NodeExprDecimal
            {
                ValueToken = tok,
                Location = tok.Location,
                Tokens = [tok]
            };
        
        if (tok.Type == TokenType.String)
            return new NodeExprStr
            {
                ValueToken = tok,
                Location = tok.Location,
                Tokens = [tok]
            };

        throw new InvalidOperationException("Unknown primitive expression");
    }
}