using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.Scanning;

public sealed record Token(TokenType Type, string Value, FileLocation Location)
{
    public bool IsEof => Type == TokenType.Eof;
}

public sealed class TokenType
{
    private TokenType(string niceName)
    {
        NiceName = niceName;
    }
    
    public string NiceName { get; set; }
    
    public static readonly TokenType IllegalCharacter = new("<illegal character>");
    public static readonly TokenType Identifier = new("name");
    public static readonly TokenType ParenLeft = new("(");
    public static readonly TokenType ParenRight = new(")");
    public static readonly TokenType BracketLeft = new("[");
    public static readonly TokenType BracketRight = new("]");
    public static readonly TokenType CurlyLeft = new("{");
    public static readonly TokenType CurlyRight = new("}");
    public static readonly TokenType Arrow = new("->");
    public static readonly TokenType Colon = new(":");
    public static readonly TokenType Semicolon = new(";");
    public static readonly TokenType OpDot = new(".");
    public static readonly TokenType Comma = new(",");
    public static readonly TokenType OpPlus = new("+");
    public static readonly TokenType OpMinus = new("-");
    public static readonly TokenType OpMul = new("*");
    public static readonly TokenType OpDiv = new("/");
    public static readonly TokenType OpSet = new("=");
    public static readonly TokenType OpEq = new("=");
    public static readonly TokenType OpNeq = new("!=");
    public static readonly TokenType OpBang = new("!");
    public static readonly TokenType Int = new("integer");
    public static readonly TokenType Decimal = new("decimal");
    public static readonly TokenType String = new("string");
    public static readonly TokenType KeywordVar = new("var");
    public static readonly TokenType KeywordRet = new("ret");
    public static readonly TokenType KeywordType = new("type");
    public static readonly TokenType KeywordEnum = new("enum");
    public static readonly TokenType Eof = new("<End of file>");

    public override string ToString()
    {
        return $"Tok<'{NiceName}'>";
    }
}