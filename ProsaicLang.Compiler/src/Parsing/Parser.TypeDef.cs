using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsTypeAlias()
    {
        return _stream.CurrentIs(TokenType.KeywordType);
    }

    private NodeTypeDef ParseTypeAlias()
    {
        Token keyword = _stream.Consume();

        if (!_stream.CurrentIs(TokenType.Identifier))
        {
            AddErrorExpectedToken(TokenType.Identifier, "type name");
        }

        Token nameTok = _stream.Consume();

        if (!_stream.CurrentIs(TokenType.OpSet))
        {
            AddErrorExpectedToken(TokenType.OpSet, "for type alias");
        }

        // alias
        _stream.Consume();

        TypeRef target;
        if (!IsTypeRef())
        {
            AddErrorExpectedToken(TokenType.Identifier, "for type alias");
            target = new TypeRefUnknown([_stream.Peek()]);
        }
        else
        {
            target = ParseTypeRef();
        }

        Sym symbol = new SymTypeAlias
        {
            TargetType = SymTypeUnresolved.CreateUnresolvedByName(target.Name, target.Location),
            Name = nameTok.Value,
            Location = nameTok.Location,
        };

        ScopedSymbolTable st = _symbolTables.Peek();
        _symbolTables.Peek().AddSymbol(symbol);

        return new NodeTypeDefAlias(nameTok.Value)
        {
            TargetType = target,
            Location = nameTok.Location,
            Tokens = _stream.GetTokenRange(keyword, _stream.Peek(-1)),
            Symbol = symbol
        };
    }
    
    private bool IsTypeStruct()
    {
        return _stream.CurrentIs(TokenType.KeywordStruct);
    }

    private NodeTypeDef ParseTypeStruct()
    {
        Token keyword = _stream.Consume();

        if (!_stream.CurrentIs(TokenType.Identifier))
        {
            AddErrorExpectedToken(TokenType.Identifier, "type name");
        }

        Token nameTok = _stream.Consume();

        if (!_stream.CurrentIs(TokenType.CurlyLeft))
        {
            AddErrorExpectedToken(TokenType.CurlyLeft, "for type definition");
        }
        
        _stream.Consume(); // {

        StructFields fields = ParseStructFields();

        Token lastTok;
        if (_stream.CurrentIs(TokenType.CurlyRight))
        {
            lastTok = _stream.Consume();
        }
        else
        {
            AddErrorExpectedToken(TokenType.CurlyRight, "to close type definition");
            lastTok = _stream.Peek(-1);
        }
            
        Sym structSym = CreateStructSymbol(nameTok.Value, fields);
        _symbolTables.Peek().AddSymbol(structSym);

        return new NodeTypeDefStructNamed(nameTok.Value)
        {
            Location = nameTok.Location,
            Tokens = _stream.GetTokenRange(keyword, lastTok),
            Symbol = structSym,
            NameToken = nameTok,
            Fields = fields
        };
    }
    
    private bool IsTypeInterface()
    {
        return _stream.CurrentIs(TokenType.KeywordInterface);
    }

    private NodeTypeDef ParseTypeInterface()
    {
        Token keyword = _stream.Consume();

        if (!_stream.CurrentIs(TokenType.Identifier))
        {
            AddErrorExpectedToken(TokenType.Identifier, "interface name");
        }

        Token nameTok = _stream.Consume();

        if (!_stream.CurrentIs(TokenType.CurlyLeft))
        {
            AddErrorExpectedToken(TokenType.CurlyLeft, "for type definition");
        }
        
        _stream.Consume(); // {

        List<InterfaceMethod> methods = [];

        while (_stream.CurrentIs(TokenType.Identifier) && !_stream.IsEof)
        {
            Token funcName = _stream.Consume();
            FuncParams funcParams = ParseFuncDefParams();
            TypeRef funcRetType = new TypeRefUnknown([funcName]);

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
            else
            {
                Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                    "No specified return type for interface method",
                    funcName.Location,
                    [funcName]
                ));
            }
            
            methods.Add(new InterfaceMethod
            {
                Tokens = _stream.GetTokenRange(nameTok, _stream.Peek(-1)),
                Location = nameTok.Location,
                Name = nameTok.Value,
                NameToken = nameTok,
                Params = funcParams,
                ReturnType = funcRetType
            });

            if (_stream.CurrentIs(TokenType.Semicolon))
            {
                _stream.Consume(); // ;
            }
            else
            {
                AddErrorExpectedSemicolon();
            }
        }

        Token lastTok;
        if (_stream.CurrentIs(TokenType.CurlyRight))
        {
            lastTok = _stream.Consume();
        }
        else
        {
            AddErrorExpectedToken(TokenType.CurlyRight, "to close interface definition");
            lastTok = _stream.Peek(-1);
        }

        Sym sym = new SymTypeInterface
        {
            Location = nameTok.Location,
            Name = nameTok.Value,
            MethodNames = methods.Select(x => x.Name).ToArray(),
            MethodParamNames = methods.Select(x => x.Params.Names.ToArray()).ToArray(),
            MethodParamTypes = methods.Select(
                x => x.Params.Types
                    .Select(p => SymTypeUnresolved.CreateUnresolvedByName(p.Name, p.Location))
                    .ToArray()
                ).ToArray(),
            MethodReturnTypes = methods.Select(
                x => SymTypeUnresolved.CreateUnresolvedByName(
                    x.ReturnType.Name,
                    x.ReturnType.Location
                )
            ).ToArray(),
        };
        _symbolTables.Peek().AddSymbol(sym);

        return new NodeTypeDefInterface(nameTok.Value)
        {
            Location = nameTok.Location,
            Tokens = _stream.GetTokenRange(keyword, lastTok),
            Symbol = sym,
            NameToken = nameTok,
            Methods = methods
        };
    }
    
    private StructFields ParseStructFields()
    {
        Token startTok = _stream.Peek();
        List<string> fieldNames = [];
        List<TypeRef> fieldTypes = [];
        List<SymTypeStruct?> innerStructs = [];

        while (_stream.CurrentIs(TokenType.Identifier))
        {
            Token fieldNameTok = _stream.Consume();

            if (!_stream.CurrentIs(TokenType.Colon))
            {
                AddErrorExpectedToken(TokenType.Colon, "for field type");
                continue;
            }

            _stream.Consume(); // :

            TypeRef fieldType;
            if (IsTypeRef())
            {
                fieldType = ParseTypeRef();
                innerStructs.Add(null);
            }
            else if (_stream.CurrentIs(TokenType.CurlyLeft))
            {
                Token openBraceTok = _stream.Consume();

                StructFields inner = ParseStructFields();

                if (_stream.IsEof)
                {
                    throw new UnexpectedEofException("struct field definition");
                }

                Token lastTok = inner.Tokens.LastOrDefault() ?? openBraceTok;
                if (!_stream.CurrentIs(TokenType.CurlyRight))
                {
                    AddErrorExpectedToken(TokenType.CurlyRight, "to close struct field definition");
                }
                else
                {
                    lastTok = _stream.Consume(); // '}'
                }
                
                List<Token> tokens = ((Token[])[openBraceTok, ..inner.Tokens, lastTok]).Distinct().ToList();
                var anonStructSymbol = CreateStructSymbol($"{{{fieldNameTok.Value}}}", inner);
                
                NodeTypeDefStructAnonymous anonStruct = new()
                {
                    Fields = inner,
                    Location = inner.Location,
                    Symbol = anonStructSymbol,
                    Tokens = tokens
                };
                
                fieldType = new TypeRefAnonymousStruct(anonStruct, tokens);
                innerStructs.Add(anonStructSymbol);
            }
            else
            {
                Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                    "Expected type for field", _stream.Peek().Location,
                    [_stream.Peek()]
                ));
                fieldType = new TypeRefUnknown([_stream.Consume()]);
                innerStructs.Add(null);
            }

            fieldNames.Add(fieldNameTok.Value);
            fieldTypes.Add(fieldType);

            if (_stream.CurrentIs(TokenType.Semicolon))
            {
                _stream.Consume();
            }
            else
            {
                AddErrorExpectedSemicolon();
            }
        }

        return new StructFields
        {
            Location = FileLocation.CreateRange(startTok.Location, _stream.Peek(-1).Location),
            Tokens = _stream.GetTokenRange(startTok, _stream.Peek(-1)),
            Names = fieldNames.ToArray(),
            Types = fieldTypes.ToArray(),
            InnerAnonymousStructs = innerStructs.ToArray()
        };
    }

    private SymTypeStruct CreateStructSymbol(string name, StructFields fields)
    {
        string[] names = fields.Names;
        SymType[] types = new SymType[fields.Types.Length];

        for (int i = 0; i < types.Length; i++)
        {
            TypeRef typeRef = fields.Types[i];
            if (typeRef is TypeRefAnonymousStruct)
            {
                SymTypeStruct inner = fields.InnerAnonymousStructs[i]!;
                types[i] = inner;
            }
            else
            {
                types[i] = SymTypeUnresolved.CreateUnresolvedByName(typeRef.Name, typeRef.Location);
            }
        }

        var symbol = new SymTypeStruct
        {
            Name = name,
            FieldNames = names,
            FieldTypes = types,
            Location = fields.Location
        };
        foreach (SymType type in types)
        {
            if (type is SymTypeStruct st)
            {
                st.ParentStruct = symbol;
            }
        }

        return symbol;
    }
}