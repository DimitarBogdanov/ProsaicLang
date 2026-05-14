using ProsaicLang.Compiler.Ast;
using ProsaicLang.Compiler.Data;
using ProsaicLang.Compiler.Symbols;

namespace ProsaicLang.Compiler.SemanticAnalysis;

public sealed class TypeResolvePass : BasePass, IVisitor
{
    public TypeResolvePass(FileUnit file, SemanticAnalyser semanticAnalyser)
        : base(file, semanticAnalyser)
    {
    }

    public void VisitTypeDefAlias(NodeTypeDefAlias typeDef)
    {
        SymType? target = FindTypeSymbol(typeDef.TargetType.Name);
        if (target == null)
        {
            Analyser.Messages.Add(new CompilerMessage(
                CompilerMessageType.Error,
                $"Could not find type '{typeDef.TargetType.Name}'",
                typeDef.TargetType.Location,
                typeDef.Tokens
            ));
            return;
        }

        ((SymTypeAlias)typeDef.Symbol).TargetType = target;
    }

    public void VisitTypeDefStructNamed(NodeTypeDefStructNamed typeDef)
    {
        SymTypeStruct structSymbol = (SymTypeStruct)typeDef.Symbol;

        for (var i = 0; i < structSymbol.FieldTypes.Length; i++)
        {
            var fieldType = structSymbol.FieldTypes[i];
            if (!fieldType.HasUnresolvedTypeReferences())
            {
                continue;
            }

            if (fieldType is SymTypeStruct)
            {
                VisitTypeDefStructAnonymous(((TypeRefAnonymousStruct)typeDef.Fields.Types[i]).StructDef);
            }
            else
            {
                var fieldTypeSymbol = FindTypeSymbol(fieldType.Name);
                if (fieldTypeSymbol == null)
                {
                    Analyser.Messages.Add(new CompilerMessage(
                        CompilerMessageType.Error,
                        $"Could not find type '{fieldType.Name}'",
                        fieldType.Location,
                        typeDef.Tokens
                    ));
                }
                else
                {
                    structSymbol.FieldTypes[i] = fieldTypeSymbol;
                }
            }
        }
    }

    public void VisitTypeDefStructAnonymous(NodeTypeDefStructAnonymous typeDef)
    {
        SymTypeStruct structSymbol = (SymTypeStruct)typeDef.Symbol;

        for (var i = 0; i < structSymbol.FieldTypes.Length; i++)
        {
            var fieldType = structSymbol.FieldTypes[i];
            if (!fieldType.HasUnresolvedTypeReferences())
            {
                continue;
            }

            if (fieldType is SymTypeStruct)
            {
                VisitTypeDefStructAnonymous(((TypeRefAnonymousStruct)typeDef.Fields.Types[i]).StructDef);
            }
            else
            {
                var fieldTypeSymbol = FindTypeSymbol(fieldType.Name);
                if (fieldTypeSymbol == null)
                {
                    Analyser.Messages.Add(new CompilerMessage(
                        CompilerMessageType.Error,
                        $"Could not find type '{fieldType.Name}'",
                        fieldType.Location,
                        typeDef.Tokens
                    ));
                }
                else
                {
                    structSymbol.FieldTypes[i] = fieldTypeSymbol;
                }
            }
        }
    }

    public void VisitFuncDef(NodeFuncDef funcDef)
    {
        if (funcDef.ReturnType != null)
        {
            var returnTypeSymbol = FindTypeSymbol(funcDef.ReturnType.Name);
            if (returnTypeSymbol == null)
            {
                Analyser.Messages.Add(new CompilerMessage(
                    CompilerMessageType.Error,
                    $"Could not find type '{funcDef.ReturnType.Name}'",
                    funcDef.ReturnType.Location,
                    funcDef.Tokens
                ));
            }
            else
            {
                ((SymFunc)funcDef.FuncSymbol).ReturnType = returnTypeSymbol;
            }
        }

        for (var i = 0; i < funcDef.Params.Types.Length; i++)
        {
            TypeRef typeRef = funcDef.Params.Types[i];
            SymType? typeSymbol = FindTypeSymbol(typeRef.Name);
            if (typeSymbol == null)
            {
                Analyser.Messages.Add(new CompilerMessage(
                    CompilerMessageType.Error,
                    $"Could not find type '{typeRef.Name}'",
                    typeRef.Location,
                    funcDef.Tokens
                ));
            }
            else
            {
                ((SymFunc)funcDef.FuncSymbol).ParamTypes[i] = typeSymbol;
            }
        }
        
        PushSymbolTable(funcDef.BodySymbolTable);
        funcDef.Body.AcceptVisitor(this);
        PopSymbolTable();
    }

    public void VisitExprAssignment(NodeExprAssignment assignExpr)
    {
        assignExpr.Target.AcceptVisitor(this);
    }

    public void VisitExprBinaryOp(NodeExprBinaryOp binaryOp)
    {
        binaryOp.Left.AcceptVisitor(this);
        binaryOp.Right.AcceptVisitor(this);
    }

    public void VisitExprUnaryOp(NodeExprUnaryOp unaryOp)
    {
        unaryOp.Expr.AcceptVisitor(this);
    }

    public void VisitExprFuncCall(NodeExprFuncCall funcCall)
    {
        funcCall.Func.AcceptVisitor(this);
        funcCall.Args.ForEach(arg => arg.AcceptVisitor(this));
    }

    public void VisitExprNameRef(NodeExprNameRef nameRef)
    {
        nameRef.Parent?.AcceptVisitor(this);
    }

    public void VisitExprBoolean(NodeExprBoolean boolean)
    {
    }

    public void VisitExprDecimal(NodeExprDecimal decimalExpr)
    {
    }

    public void VisitExprInt(NodeExprInt intExpr)
    {
    }

    public void VisitExprStr(NodeExprStr intExpr)
    {
    }

    public void VisitStatAssignment(NodeStatAssignment assignExpr)
    {
        assignExpr.AssignExpr.AcceptVisitor(this);
    }

    public void VisitStatBlock(NodeStatBlock block)
    {
        block.Children.ForEach(child => child.AcceptVisitor(this));
    }

    public void VisitStatFuncCall(NodeStatFuncCall funcCall)
    {
        VisitExprFuncCall(funcCall.FuncCallExpr);
    }

    public void VisitStatVarDecl(NodeStatVarDecl varDecl)
    {
        SymType? type = FindTypeSymbol(varDecl.SpecifiedType.Name);
        if (type == null)
        {
            Analyser.Messages.Add(new CompilerMessage(
                CompilerMessageType.Error,
                $"Could not find type '{varDecl.SpecifiedType.Name}'",
                varDecl.SpecifiedType.Location,
                varDecl.Tokens
            ));
        }
        else
        {
            varDecl.Symbol.Type = type;
        }
        
        varDecl.Initialiser?.AcceptVisitor(this);
    }

    public void VisitStatNoOperation()
    {
    }
}