using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    private bool IsStat()
    {
        return IsStatBlock()
               || IsStatNoOperation();
    }

    private NodeStat ParseStat()
    {
        if (IsStatBlock())
        {
            return ParseStatBlock();
        }

        if (IsStatNoOperation())
        {
            return ParseStatNoOperation();
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
                statements.Add(ParseStat());
            }
            else
            {
                Messages.Add(new CompilerMessage(CompilerMessageType.Error,
                    "Expected statement", _stream.Peek().Location,
                    [_stream.Peek()]
                ));
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
}