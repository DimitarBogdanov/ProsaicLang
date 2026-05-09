namespace ProsaicLang.Compiler.Parsing;

public sealed class UnexpectedEofException : Exception
{
    public UnexpectedEofException(string whileParsing)
        : base($"Unexpected end of file while parsing {whileParsing}.")
    {
    }
}