using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsTypeDef()
    {
        return _stream.CurrentIs(TokenType.KeywordType);
    }

    private NodeTypeDef ParseTypeDef()
    {
        Token keyword = _stream.Consume();

        if (!_stream.CurrentIs(TokenType.Identifier))
        {
            AddErrorExpectedToken(TokenType.Identifier, "type name");
        }

        Token nameTok = _stream.Consume();

        if (_stream.CurrentIs(TokenType.OpSet))
        {
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
                TargetType = new SymTypeUnresolved
                {
                    Location = target.Location,
                    Name = target.Name
                },
                Name = nameTok.Value,
                Location = nameTok.Location,
            };
            
            _symbolTables.Peek().AddSymbol(symbol);

            return new NodeTypeDefAlias(nameTok.Value)
            {
                TargetType = target,
                Location = nameTok.Location,
                Tokens = _stream.GetTokenRange(keyword, _stream.Peek(-1)),
                Symbol = symbol
            };
        }

        if (_stream.CurrentIs(TokenType.CurlyLeft))
        {
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

        throw new InvalidOperationException("Unknown type definition to parse");
    }

    private StructFields ParseStructFields()
    {
        Token startTok = _stream.Peek();
        List<string> fieldNames = [];
        List<TypeRef> fieldTypes = [];

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
                
                Sym anonStructSym = CreateStructSymbol($"{{{fieldNameTok.Value}}}", inner);

                List<Token> tokens = ((Token[])[openBraceTok, ..inner.Tokens, lastTok]).Distinct().ToList();
                NodeTypeDefStructAnonymous anonStruct = new()
                {
                    Fields = inner,
                    Location = inner.Location,
                    Symbol = anonStructSym,
                    Tokens = tokens
                };
                fieldType = new TypeRefAnonymousStruct(anonStruct, tokens);
            }
            else
            {
                Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                    "Expected type for field", _stream.Peek().Location,
                    [_stream.Peek()]
                ));
                fieldType = new TypeRefUnknown([_stream.Consume()]);
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
            Types = fieldTypes.ToArray()
        };
    }

    private SymTypeStruct CreateStructSymbol(string name, StructFields fields)
    {
        string[] names = fields.Names;
        SymType[] types = new SymType[fields.Types.Length];

        for (int i = 0; i < types.Length; i++)
        {
            TypeRef typeRef = fields.Types[i];
            if (typeRef is TypeRefAnonymousStruct anonStruct)
            {
                SymTypeStruct inner = CreateStructSymbol($"{{{names[i]}}}", anonStruct.StructDef.Fields);
                types[i] = inner;
            }
            else
            {
                types[i] = new SymTypeUnresolved
                {
                    Name = typeRef.Name,
                    Location = typeRef.Location
                };
            }
        }

        return new SymTypeStruct
        {
            Name = name,
            FieldNames = names,
            FieldTypes = types,
            Location = fields.Location
        };
    }
}