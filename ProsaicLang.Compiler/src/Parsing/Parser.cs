using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    public Parser(string fileName, List<Token> tokens)
    {
        _fileName = fileName;
        _stream = new TokenStream(fileName, tokens);
        Messages = [];
    }
    
    public List<CompilerMessage> Messages { get; }
    
    private readonly string _fileName;
    private readonly TokenStream _stream;

    public void Run()
    {
        while (!_stream.IsEof)
        {
            if (IsFuncDef())
            {
                var x = ParseFuncDef();
                continue;
            }

            Messages.Add(new CompilerMessage(
                CompilerMessageType.Error,
                "Unexpected token",
                _stream.Peek().Location,
                [_stream.Peek()]
            ));
            break;
        }
    }

    private bool IsTypeRef()
    {
        return _stream.CurrentIs(TokenType.Identifier);
    }

    private TypeRef ParseTypeRef()
    {
        Token tok = _stream.Consume();
        
        TypeRef current = new TypeRefNamed(tok.Value, [tok]);
        while (_stream.CurrentIs(TokenType.BracketLeft) && _stream.NextIs(TokenType.BracketRight))
        {
            var open = _stream.Consume();
            var close = _stream.Consume();
            current = new TypeRefArr(current, [..current.Tokens, open, close]);
        }

        return current;
    }

    private void AddErrorExpectedToken(TokenType expected, string whyIsItExpected)
    {
        Messages.Add(new CompilerMessage(
            CompilerMessageType.Error,
            $"Expected {expected.NiceName} ({whyIsItExpected}), got '{_stream.Peek().Value}'",
            _stream.Peek().Location,
            [_stream.Peek()]
        ));   
    }
    
    private void AddErrorExpectedSemicolon()
    {
        Token lastToken = _stream.Peek(-1);

        FileLocation lastCharacter = new FileLocation(
            lastToken.Location.FileName,
            lastToken.Location.EndLine,
            lastToken.Location.EndColumn,
            lastToken.Location.EndLine,
            lastToken.Location.EndColumn
        );
        
        Messages.Add(new CompilerMessage(
            CompilerMessageType.Error,
            "Expected semicolon",
            lastCharacter,
            [_stream.Peek(-1), _stream.Peek()]
        ));   
    }
}