using System.Text;
using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.Scanning;

public sealed class Lexer
{
    public Lexer(string fileName)
    {
        _fileName = fileName;
        Messages = [];
        Tokens = [];
    }
    
    public List<CompilerMessage> Messages { get; }
    public List<Token> Tokens { get; }
    
    private readonly string _fileName;
    
    public void Run(Stream stream)
    {
        using StreamReader reader = new(stream);

        int line = 1;
        int col = 0;
        
        StringBuilder sb = new(12);
        
        while (!reader.EndOfStream)
        {
            int current = reader.Read();
            if (current == -1)
            {
                break;
            }

            if (current is '\n')
            {
                line++;
                col = 0;
                continue;
            }

            col++;

            if (Char.IsControl((char)current))
            {
                continue;
            }

            if (current is '.' or >= '0' and <= '9')
            {
                TokenType type = current == '.' ? TokenType.Decimal : TokenType.Int;
                int colStart = col;
                bool hasWholePart = false;
                if (type == TokenType.Int)
                {
                    hasWholePart = true;
                    sb.Append((char)current);
                    while ((current = reader.Peek()) is >= '0' and <= '9')
                    {
                        reader.Read();
                        sb.Append((char)current);
                        col++;
                    }
                }

                if (current is '.')
                {
                    type = TokenType.Decimal;
                    if (hasWholePart) reader.Read();
                    if (sb.Length == 0)
                    {
                        sb.Append('0');
                    }
                    sb.Append((char)current);
                    col++;
                    while ((current = reader.Peek()) is >= '0' and <= '9')
                    {
                        reader.Read();
                        sb.Append((char)current);
                        col++;
                    }
                }
                
                AddTok(type, sb.ToString(), line, colStart, line, hasWholePart ? col : col - 1);
                sb.Clear();
                
                continue;
            }
            
            // if we reached here, then we didn't early exit
            AddTok(TokenType.IllegalCharacter, ((char)current).ToString(), line, col);
        }
    }

    private void AddTok(TokenType type, string value, int lineStart, int colStart, int lineEnd, int colEnd)
    {
        FileLocation loc = new(_fileName, lineStart, colStart, lineEnd, colEnd);       
        Tokens.Add(new Token(type, value, loc));
    }

    private void AddTok(TokenType type, string value, int lineStart, int colStart)
    {
        AddTok(type, value, lineStart, colStart, lineStart, colStart + value.Length - 1);
    }

    private void AddUnknownError(Token tok)
    {
        Messages.Add(new CompilerMessage(CompilerMessageType.Error, "Unknown token.", tok.Location, [tok]));
    }
}