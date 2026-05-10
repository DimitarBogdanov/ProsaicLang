using System.Text;
using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Scanning;

namespace ProsaicLang.Compiler.Parsing;

public partial class Parser
{
    public Parser(string fileName, List<Token> tokens)
    {
        _fileName = fileName;
        _stream = new TokenStream(fileName, tokens);
        Messages = [];
    }
    
    public List<CompilerMessage> Messages { get; }
    
    private readonly string _fileName;
    private readonly TokenStream _stream;

    public void Run()
    {
        List<NodeFuncDef> funcDefs = [];
        List<NodeTypeDef> typeDefs = [];
        List<ModuleNameRef> imports = [];
        ModuleNameRef? moduleName = null;
        
        while (!_stream.IsEof)
        {
            try
            {
                if (IsTypeDef())
                {
                    typeDefs.Add(ParseTypeDef());
                    continue;
                }

                if (IsFuncDef())
                {
                    funcDefs.Add(ParseFuncDef());
                    continue;
                }

                if (IsImport())
                {
                    var mod = ParseImport();
                    imports.Add(mod);
                    
                    if (funcDefs.Count != 0 || typeDefs.Count != 0 || moduleName != null)
                    {
                        Messages.Add(new CompilerMessage(
                            CompilerMessageType.Error,
                            "Imports should be at the top of the file",
                            mod.Location,
                            mod.Tokens
                        ));
                    }
                    
                    continue;
                }

                if (IsModuleDecl())
                {
                    var mod = ParseModuleDecl();
                    if (moduleName != null)
                    {
                        Messages.Add(new CompilerMessage(
                            CompilerMessageType.Error,
                            "Module has already been declared above",
                            mod.Location,
                            mod.Tokens
                        ));
                        continue;
                    }

                    if (funcDefs.Count != 0 || typeDefs.Count != 0)
                    {
                        Messages.Add(new CompilerMessage(
                            CompilerMessageType.Error,
                            "Module declaration must be after imports and before any other declarations",
                            mod.Location,
                            mod.Tokens
                        ));
                    }

                    moduleName = mod;
                    continue;
                }
            }
            catch (ParsingMustRecoverException)
            {
                Token[] parsedTokens = _stream.GetTokensUntilCurrentPosition();
                int openedBraces = parsedTokens.Count(t => t.Type == TokenType.CurlyLeft);
                int closedBraces = parsedTokens.Count(t => t.Type == TokenType.CurlyRight);
                if (openedBraces == closedBraces)
                {
                    _stream.Consume();
                    continue;
                }
                
                int remainingBraces = openedBraces - closedBraces;
                if (remainingBraces > 0)
                {
                    while (!_stream.CurrentIs(TokenType.CurlyRight) && !_stream.IsEof)
                    {
                        _stream.Consume();
                    }
                }

                _stream.Consume();
                continue;
            }

            Messages.Add(new CompilerMessage(
                CompilerMessageType.Error,
                "Unexpected token",
                _stream.Peek().Location,
                [_stream.Peek()]
            ));
            break;
        }
    }

    private bool IsTypeRef()
    {
        return _stream.CurrentIs(TokenType.Identifier);
    }

    private TypeRef ParseTypeRef()
    {
        Token tok = _stream.Consume();
        
        TypeRef current = new TypeRefNamed(tok.Value, [tok]);
        while (_stream.CurrentIs(TokenType.BracketLeft) && _stream.NextIs(TokenType.BracketRight))
        {
            var open = _stream.Consume();
            var close = _stream.Consume();
            current = new TypeRefArr(current, [..current.Tokens, open, close]);
        }

        return current;
    }

    private bool IsImport()
    {
        return _stream.CurrentIs(TokenType.KeywordImport);
    }

    private ModuleNameRef ParseImport()
    {
        Token keyword = _stream.Consume();
        
        if (!_stream.CurrentIs(TokenType.Identifier))
        {
            Messages.Add(new CompilerMessage(
                CompilerMessageType.Error,
                "Expected module name",
                FileLocation.CreateRange(keyword.Location, _stream.Peek().Location),
                [keyword, _stream.Peek()]
            ));
            return new ModuleNameRef
            {
                Tokens = [keyword],
                Location = keyword.Location,
                Name = "<???>"
            };
        }
        
        Token moduleNameTok = _stream.Consume();

        StringBuilder sb = new(moduleNameTok.Value);
        while (_stream.CurrentIs(TokenType.DoubleColon))
        {
            _stream.Consume();
            Token nextTok = _stream.Consume();
            if (nextTok.Type != TokenType.Identifier)
            {
                Messages.Add(new CompilerMessage(
                    CompilerMessageType.Error,
                    "Expected name after ::",
                    nextTok.Location,
                    [nextTok]
                ));

                if (_stream.Peek().Location.StartLine != keyword.Location.StartLine)
                {
                    // probably just a missing semicolon or something
                    throw new ParsingMustRecoverException();
                }
            }

            sb.Append(':', 2);
            sb.Append(nextTok.Value);
        }

        if (_stream.CurrentIs(TokenType.Semicolon))
        {
            _stream.Consume();
        }
        else
        {
            AddErrorExpectedSemicolon();
        }

        List<Token> tokens = _stream.GetTokenRange(keyword, _stream.Peek(-1));
        return new ModuleNameRef
        {
            Tokens = tokens,
            Location = FileLocation.CreateRange(keyword.Location, _stream.Peek(-1).Location),
            Name = sb.ToString()
        };
    }
    
    private bool IsModuleDecl()
    {
        return _stream.CurrentIs(TokenType.KeywordModule);
    }

    private ModuleNameRef ParseModuleDecl()
    {
        Token keyword = _stream.Consume();
        
        if (!_stream.CurrentIs(TokenType.Identifier))
        {
            Messages.Add(new CompilerMessage(
                CompilerMessageType.Error,
                "Expected module name",
                FileLocation.CreateRange(keyword.Location, _stream.Peek().Location),
                [keyword, _stream.Peek()]
            ));
            return new ModuleNameRef
            {
                Tokens = [keyword],
                Location = keyword.Location,
                Name = "<???>"
            };
        }
        
        Token moduleNameTok = _stream.Consume();

        StringBuilder sb = new(moduleNameTok.Value);
        while (_stream.CurrentIs(TokenType.DoubleColon))
        {
            _stream.Consume();
            Token nextTok = _stream.Consume();
            if (nextTok.Type != TokenType.Identifier)
            {
                Messages.Add(new CompilerMessage(
                    CompilerMessageType.Error,
                    "Expected name after ::",
                    nextTok.Location,
                    [nextTok]
                ));

                if (_stream.Peek().Location.StartLine != keyword.Location.StartLine)
                {
                    // probably just a missing semicolon or something
                    throw new ParsingMustRecoverException();
                }
            }

            sb.Append(':', 2);
            sb.Append(nextTok.Value);
        }

        if (_stream.CurrentIs(TokenType.Semicolon))
        {
            _stream.Consume();
        }
        else
        {
            AddErrorExpectedSemicolon();
        }

        List<Token> tokens = _stream.GetTokenRange(keyword, _stream.Peek(-1));
        return new ModuleNameRef
        {
            Tokens = tokens,
            Location = FileLocation.CreateRange(keyword.Location, _stream.Peek(-1).Location),
            Name = sb.ToString()
        };
    }

    private void AddErrorExpectedToken(TokenType expected, string whyIsItExpected)
    {
        Messages.Add(new CompilerMessage(
            CompilerMessageType.Error,
            $"Expected {expected.NiceName} ({whyIsItExpected}), got '{_stream.Peek().Value}'",
            _stream.Peek().Location,
            [_stream.Peek()]
        ));   
    }
    
    private void AddErrorExpectedSemicolon()
    {
        Token lastToken = _stream.Peek(-1);

        FileLocation lastCharacter = new FileLocation(
            lastToken.Location.FileName,
            lastToken.Location.EndLine,
            lastToken.Location.EndColumn,
            lastToken.Location.EndLine,
            lastToken.Location.EndColumn
        );
        
        Messages.Add(new CompilerMessage(
            CompilerMessageType.Error,
            "Expected semicolon",
            lastCharacter,
            [_stream.Peek(-1), _stream.Peek()]
        ));   
    }
}