using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

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

            return new NodeTypeDefAlias(nameTok.Value)
            {
                TargetType = target,
                Location = nameTok.Location,
                Tokens = _stream.GetTokenRange(keyword, _stream.Peek(-1))
            };
        }

        throw new InvalidOperationException("Unknown type definition to parse");
    }
}