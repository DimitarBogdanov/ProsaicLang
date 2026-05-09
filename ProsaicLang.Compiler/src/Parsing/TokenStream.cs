using System.Diagnostics.CodeAnalysis;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public sealed class TokenStream
{
    public TokenStream(string fileName, List<Token> tokens)
    {
        _tokens = new List<Token>(tokens);
        Token? last = _tokens.LastOrDefault();
        FileLocation eof = last?.Location ?? new FileLocation(fileName, 0, 0);
        _tokens.Add(_eofTok = new Token(TokenType.Eof, TokenType.Eof.NiceName, eof));
        _rememberedPositions = [];
    }
    
    private readonly List<Token> _tokens;
    private readonly Stack<int> _rememberedPositions;
    private readonly Token _eofTok;
    private int _pos = 0;

    public Token Peek() => _tokens[_pos];
    public Token Peek(int offset) => (_pos + offset) >= _tokens.Count ? _eofTok : _tokens[_pos + offset];
    
    public Token Consume()
    {
        if (_pos >= _tokens.Count)
            return _eofTok;
        return _tokens[_pos++];
    }

    public bool IsEof => Peek().IsEof;
    public bool CurrentIs(TokenType type) => Peek().Type == type;
    public bool NextIs(TokenType type) => Peek(1).Type == type;

    public List<Token> GetTokenRange(Token startInclusive, Token endInclusive)
    {
        int start = _tokens.IndexOf(startInclusive);
        int end = _tokens.IndexOf(endInclusive);
        return _tokens.GetRange(start, end - start + 1);
    }
    
    public void RememberPosition() => _rememberedPositions.Push(_pos);
    public void RestorePosition() => _pos = _rememberedPositions.Pop();
}