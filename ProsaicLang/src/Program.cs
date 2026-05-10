using System.Text;
using ProsaicLang.Compiler.Parsing;
using ProsaicLang.Compiler.Scanning;

string source = """
                import std;
                module app;
                type MyInt = Int32
                type WeatherReport {
                    Europe: Winning;
                    America: Losing;
                    China: {
                        It: Depends;
                    };
                }
                main(args: Str[]) -> Int {
                    var x = a;
                    a = b;
                    a.x = c;
                }
                """;
using MemoryStream ms = new(Encoding.UTF8.GetBytes(source));
Lexer lexer = new("test.pl");
lexer.Run(ms);
// lexer.Tokens.ForEach(Console.WriteLine);
lexer.Messages.ForEach(Console.WriteLine);

Parser parser = new("test.pl", lexer.Tokens);
var file = parser.Run();
parser.Messages.ForEach(Console.WriteLine);

