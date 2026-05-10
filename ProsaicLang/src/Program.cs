using System.Text;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Parsing;
using ProsaicLang.Compiler.Scanning;
using ProsaicLang.Compiler.SemanticAnalysis;

string source = """
                import std;
                module app;
                type Value {}
                type Int = Value
                type Str = Value
                
                type Celsius = Int
                type WeatherReport {
                    temp: Celsius;
                    feelsLike: Str;
                    detailed: {
                        city: WeatherReport;
                    };
                }
                type A { b: B; }
                type B = A
                main(args: Str[]) -> Int {
                    var x: WeatherReport[];
                    x.temp = 10;
                }
                """;
using MemoryStream ms = new(Encoding.UTF8.GetBytes(source));
Lexer lexer = new("test.pl");
lexer.Run(ms);
lexer.Messages.ForEach(Console.WriteLine);

Parser parser = new("test.pl", lexer.Tokens);
var file = parser.Run();
parser.Messages.ForEach(Console.WriteLine);
if (parser.Messages.Any(x => x.Type == CompilerMessageType.Error)) return;

SemanticAnalyser analyser = new(file);
analyser.Run();
analyser.Messages.ForEach(Console.WriteLine);
