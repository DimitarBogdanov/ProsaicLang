using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsStat()
    {
        return IsStatBlock()
               || IsStatNoOperation()
               || IsStatVarDecl()
               || IsExpr();
    }

    private NodeStat? ParseStat()
    {
        if (IsStatBlock())
        {
            return ParseStatBlock();
        }

        if (IsStatNoOperation())
        {
            return ParseStatNoOperation();
        }

        if (IsStatVarDecl())
        {
            return ParseStatVarDecl();
        }

        if (IsExpr())
        {
            var value = ParseExpr();
            if (value is NodeExprFuncCall funcCall)
            {
                Token? semicolon = null;
                if (_stream.CurrentIs(TokenType.Semicolon))
                {
                    semicolon = _stream.Consume();
                }
                else
                {
                    AddErrorExpectedSemicolon();
                }

                return new NodeStatFuncCall(funcCall)
                {
                    Location = funcCall.Location,
                    Tokens = semicolon != null ? [..funcCall.Tokens, semicolon] : funcCall.Tokens,
                };
            }
            
            if (value is NodeExprAssignment assignment)
            {
                Token? semicolon = null;
                if (_stream.CurrentIs(TokenType.Semicolon))
                {
                    semicolon = _stream.Consume();
                }
                else
                {
                    AddErrorExpectedSemicolon();
                }

                return new NodeStatAssignment(assignment)
                {
                    Location = assignment.Location,
                    Tokens = semicolon != null ? [..assignment.Tokens, semicolon] : assignment.Tokens,
                };
            }

            Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                "Expected statement",
                value.Location, value.Tokens
            ));
            return null;
        }

        throw new InvalidOperationException("Unknown statement to parse");
    }

    private bool IsStatBlock()
    {
        return _stream.CurrentIs(TokenType.CurlyLeft);
    }
    
    private NodeStat ParseStatBlock()
    {
        Token openBrace = _stream.Consume();

        List<NodeStat> statements = [];

        while (!_stream.CurrentIs(TokenType.CurlyRight) && !_stream.IsEof)
        {
            if (IsStat())
            {
                var stat = ParseStat();
                if (stat != null)
                    statements.Add(stat);
            }
            else
            {
                Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                    "Expected statement", _stream.Peek().Location,
                    [_stream.Peek()]
                ));
                throw new ParsingMustRecoverException();
            }
        }

        Token closeBrace;
        if (_stream.CurrentIs(TokenType.CurlyRight))
        {
            closeBrace = _stream.Consume(); // }
        }
        else
        {
            throw new UnexpectedEofException("func body");
        }

        return new NodeStatBlock
        {
            Children = statements,
            Location = openBrace.Location,
            Tokens = _stream.GetTokenRange(openBrace, closeBrace)
        };
    }

    private bool IsStatNoOperation()
    {
        return _stream.CurrentIs(TokenType.Semicolon);
    }

    private NodeStat ParseStatNoOperation()
    {
        Token tok = _stream.Consume();
        return new NodeStatNoOperation
        {
            Location = tok.Location,
            Tokens = [tok]
        };
    }

    private bool IsStatVarDecl()
    {
        return _stream.CurrentIs(TokenType.KeywordVar);
    }

    private NodeStat ParseStatVarDecl()
    {
        Token keywordToken = _stream.Consume();
        
        if (!_stream.CurrentIs(TokenType.Identifier))
        {
            AddErrorExpectedToken(TokenType.Identifier, "for variable name");
        }
        
        Token nameTok = _stream.Consume();
        
        TypeRef? specifiedType = null;
        if (_stream.CurrentIs(TokenType.Colon))
        {
            _stream.Consume(); // :
            if (IsTypeRef())
            {
                specifiedType = ParseTypeRef();
            }
            else
            {
                AddErrorExpectedToken(TokenType.Identifier, "for variable type (e.g. var x: Int)");
            }
        }
        
        NodeExpr? initExpr = null;
        if (_stream.CurrentIs(TokenType.OpSet))
        {
            Token opSet = _stream.Consume();
            if (!IsExpr())
            {
                Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                    "Expected value for variable",
                    opSet.Location,
                    [opSet, _stream.Peek()]
                ));
            }
            else
            {
                initExpr = ParseExpr();
            }
        }

        if (_stream.CurrentIs(TokenType.Semicolon))
        {
            _stream.Consume();
        }
        else
        {
            AddErrorExpectedSemicolon();
        }

        return new NodeStatVarDecl
        {
            NameToken = nameTok,
            SpecifiedType = specifiedType,
            Initializer = initExpr,
            Location = nameTok.Location,
            Tokens = _stream.GetTokenRange(keywordToken, _stream.Peek(-1))
        };
    }
}