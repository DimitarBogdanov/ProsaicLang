using System.Text;
using ProsaicLang.Compiler.Parsing;
using ProsaicLang.Compiler.Scanning;

string source = """
                import std;
                module app;
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
parser.Run();
parser.Messages.ForEach(Console.WriteLine);

