using System.Text;
using ProsaicLang.Compiler.Scanning;

Lexer lexer = new("test.pl");
string source = """
                main(args: Str[]) -> Int { 
                    var x = "test";
                }
                """;
using MemoryStream ms = new(Encoding.UTF8.GetBytes(source));
lexer.Run(ms);
lexer.Tokens.ForEach(Console.WriteLine);