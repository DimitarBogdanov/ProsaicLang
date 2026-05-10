using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsFuncDef()
    {
        return _stream.CurrentIs(TokenType.Identifier)
               && _stream.NextIs(TokenType.ParenLeft);
    }
    
    private NodeFuncDef ParseFuncDef()
    {
        Token nameTok = _stream.Consume();
        FuncParams funcParams = ParseFuncDefParams();
        TypeRef? funcRetType = null;

        if (_stream.CurrentIs(TokenType.Arrow))
        {
            _stream.Consume();
            if (!IsTypeRef())
            {
                AddErrorExpectedToken(TokenType.Identifier, "for func return type");
            }
            else
            {
                funcRetType = ParseTypeRef();
            }
        }

        NodeStat body;
        if (!IsStat())
        {
            Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                "Expected statement or func body",
                _stream.Peek().Location,
                [_stream.Peek()]
            ));
            body = new NodeStatBlock
            {
                Children = [],
                Location = nameTok.Location,
                Tokens = [nameTok]
            };
        }
        else
        {
            body = ParseStat() ?? new NodeStatNoOperation
            {
                Location = nameTok.Location,
                Tokens = [nameTok]
            };
        }

        return new NodeFuncDef
        {
            NameToken = nameTok,
            ReturnType = funcRetType,
            Params = funcParams,
            Body = body,
            Location = nameTok.Location,
            Tokens = _stream.GetTokenRange(nameTok, body.Tokens.Last()),
        };
    }

    private FuncParams ParseFuncDefParams()
    {
        if (!_stream.CurrentIs(TokenType.ParenLeft))
        {
            AddErrorExpectedToken(TokenType.ParenLeft, "to start func parameter list");
            return new FuncParams { Names = [], Types = [] };
        }

        _stream.Consume();

        List<(string Name, TypeRef Type)> funcParams = [];
        bool first = true;
        while (!_stream.CurrentIs(TokenType.ParenRight))
        {
            if (_stream.IsEof)
            {
                throw new UnexpectedEofException("func parameters");
            }

            if (_stream.CurrentIs(TokenType.Comma))
            {
                if (first)
                {
                    Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                        "Parameters should not start with a comma",
                        _stream.Peek().Location,
                        [_stream.Peek()]
                    ));
                }

                _stream.Consume();
            }

            if (_stream.NextIs(TokenType.Identifier))
            {
                AddErrorExpectedToken(TokenType.Identifier, "of parameter");
            }
            
            Token name = _stream.Consume();
            TypeRef type;
            
            if (_stream.CurrentIs(TokenType.Comma))
            {
                AddErrorExpectedToken(TokenType.Colon, "for parameter type, e.g. x: Int");
                _stream.Consume();
                continue;
            }

            if (_stream.CurrentIs(TokenType.Colon))
            {
                _stream.Consume();
            }
            else
            {
                AddErrorExpectedToken(TokenType.Colon, "for parameter type, e.g. x: Int");
            }

            if (!IsTypeRef())
            {
                type = new TypeRefUnknown([_stream.Peek()]);
                AddErrorExpectedToken(TokenType.Identifier, "parameter type");
            }
            else
            {
                type = ParseTypeRef();
            }
            
            funcParams.Add((name.Value, type));
        }

        if (_stream.CurrentIs(TokenType.ParenRight))
        {
            _stream.Consume();
        }
        else
        {
            AddErrorExpectedToken(TokenType.ParenRight, "to close func parameter list");
        }

        return new FuncParams
        {
            Names = funcParams.Select(x => x.Name).ToArray(),
            Types = funcParams.Select(x => x.Type).ToArray(),
        };
    }
}