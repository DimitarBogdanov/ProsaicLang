using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsExpr()
    {
        return IsExprPrimitive()
            || IsExprNameRef();
    }

    private NodeExpr ParseExpr()
    {
        if (IsExprPrimitive())
            return ParseExprPrimitive();
        if (IsExprNameRef())
            return ParseExprNameRef();

        throw new InvalidOperationException("Unknown expression to parse");
    }

    private bool IsExprPrimitive()
    {
        return _stream.CurrentIs(TokenType.Int)
            || _stream.CurrentIs(TokenType.Decimal)
            || _stream.CurrentIs(TokenType.String)
            || _stream.CurrentIs(TokenType.KeywordTrue)
            || _stream.CurrentIs(TokenType.KeywordFalse);
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

        if (tok.Type == TokenType.KeywordTrue || tok.Type == TokenType.KeywordFalse)
            return new NodeExprBoolean
            {
                ValueToken = tok,
                Location = tok.Location,
                Tokens = [tok]
            };

        throw new InvalidOperationException("Unknown primitive expression");
    }

    private bool IsExprNameRef()
    {
        return _stream.CurrentIs(TokenType.Identifier);
    }

    private NodeExpr ParseExprNameRef()
    {
        Token firstId = _stream.Consume();
        
        NodeExprNameRef nameRef = new(null, firstId.Value)
        {
            Location = firstId.Location,
            Tokens = [firstId]
        };

        while (_stream.CurrentIs(TokenType.OpDot))
        {
            Token dotToken = _stream.Consume();

            Token nameTok = _stream.Consume();
            if (nameTok.Type != TokenType.Identifier)
            {
                AddErrorExpectedToken(TokenType.Identifier, "for accessor name");
            }

            nameRef = new NodeExprNameRef(nameRef, nameTok.Value)
            {
                Location = FileLocation.CreateRange(firstId.Location, nameTok.Location),
                Tokens = [..nameRef.Tokens, dotToken, nameTok]
            };
        }

        return nameRef;
    }
}