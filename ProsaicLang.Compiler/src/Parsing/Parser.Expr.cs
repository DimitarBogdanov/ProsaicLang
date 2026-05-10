using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsExpr()
    {
        return IsExprPrimitive()
            || IsExprNameRef()
            || IsExprParen()
            || IsExprUnaryOp();
    }

    private NodeExpr ParseExpr()
    {
        NodeExpr value;
        
        if (IsExprPrimitive())
            value = ParseExprPrimitive();
        else if (IsExprNameRef())
            value = ParseExprNameRef();
        else if (IsExprParen())
            value = ParseExprParen();
        else if (IsExprUnaryOp())
            value = ParseExprUnaryOp();
        else
            throw new InvalidOperationException("Unknown expression to parse");

        while (_stream.CurrentIs(TokenType.ParenLeft))
        {
            // function call
            List<NodeExpr> args = ParseExprFuncCallArgs();
            Token lastToken = _stream.Peek(-1);
            
            value = new NodeExprFuncCall(value, args)
            {
                Location = FileLocation.CreateRange(value.Location, lastToken.Location),
                Tokens = [..value.Tokens, ..args.Select(a => a.Tokens).SelectMany(t => t), lastToken]
            };
        }
        
        return value;
    }

    private bool IsExprParen()
    {
        return _stream.CurrentIs(TokenType.ParenLeft);
    }

    private NodeExpr ParseExprParen()
    {
        Token open = _stream.Consume();

        if (!IsExpr())
        {
            Messages.Add(new CompilerMessage(CompilerMessageType.Error, "Expected expression",
                open.Location, [open, _stream.Peek()]
            ));
            throw new ParsingMustRecoverException();
        }
        
        NodeExpr inner = ParseExpr();

        Token? close = null;
        if (!_stream.CurrentIs(TokenType.ParenRight))
        {
            AddErrorExpectedToken(TokenType.ParenRight, "to close expression");
        }
        else
        {
            close = _stream.Consume();
        }
        
        inner.Tokens.Insert(0, open);
        if (close != null)
            inner.Tokens.Add(close);
        
        return inner;
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

    private bool IsExprUnaryOp()
    {
        return _stream.CurrentIs(TokenType.OpMinus)
            || _stream.CurrentIs(TokenType.OpPlus)
            || _stream.CurrentIs(TokenType.OpBang);
    }

    private NodeExpr ParseExprUnaryOp()
    {
        Token opToken = _stream.Consume();
        ExprUnaryOpType type;
        if (opToken.Type == TokenType.OpMinus)
            type = ExprUnaryOpType.Negate;
        else if (opToken.Type == TokenType.OpPlus)
            type = ExprUnaryOpType.AbsoluteValue;
        else if (opToken.Type == TokenType.OpBang)
            type = ExprUnaryOpType.Not;
        else
            throw new InvalidOperationException("Unknown unary operator");

        if (!IsExpr())
        {
            Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                "Expected expression",
                opToken.Location, [opToken]
            ));
            throw new ParsingMustRecoverException();
        }
        
        var value = ParseExpr();

        return new NodeExprUnaryOp(value, type)
        {
            Location = FileLocation.CreateRange(opToken.Location, value.Location),
            Tokens = [opToken, ..value.Tokens]
        };
    }
    
    private List<NodeExpr> ParseExprFuncCallArgs()
    {
        Token openParen = _stream.Consume();

        List<NodeExpr> args = [];
        while (IsExpr())
        {
            var value = ParseExpr();
            args.Add(value);
            if (!_stream.CurrentIs(TokenType.Comma))
            {
                if (IsExpr())
                {
                    AddErrorExpectedToken(TokenType.Comma, "to separate call arguments");
                    continue;
                }

                break;
            }

            _stream.Consume(); // ,
        }

        if (_stream.CurrentIs(TokenType.ParenRight))
        {
            _stream.Consume();
        }
        else
        {
            AddErrorExpectedToken(TokenType.ParenRight, "to close call arguments");
        }

        return args;
    }
}