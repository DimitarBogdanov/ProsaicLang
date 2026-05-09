using System.Text;
using ProsaicLang.Compiler.Scanning;

Lexer lexer = new("test.pl");
string source = """
                123
                456.1
                .123
                0.1
                5
                """;
using MemoryStream ms = new(Encoding.UTF8.GetBytes(source));
lexer.Run(ms);
lexer.Tokens.ForEach(Console.WriteLine);