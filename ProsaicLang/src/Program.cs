using System.Text;
using ProsaicLang.Compiler.Scanning;

Lexer lexer = new("test.pl");
string source = """
                main(args: Str[]) -> Int {
                    var x = 2;
                    var y = 6;
                    print(x+y);
                    ret 0;
                }
                """;
using MemoryStream ms = new(Encoding.UTF8.GetBytes(source));
lexer.Run(ms);
lexer.Tokens.ForEach(Console.WriteLine);