using ProsaicLang.Compiler.Data;

namespace ProsaicLang.Compiler.Ast;

public interface IVisitor
{
    // Top level
    public void VisitTypeDefAlias(NodeTypeDefAlias typeDef);
    public void VisitTypeDefStructNamed(NodeTypeDefStructNamed typeDef);
    public void VisitTypeDefStructAnonymous(NodeTypeDefStructAnonymous typeDef);
    public void VisitTypeDefInterface(NodeTypeDefInterface typeDef);
    public void VisitFuncDef(NodeFuncDef funcDef);
    
    // Expressions
    public void VisitExprAssignment(NodeExprAssignment assignExpr);
    public void VisitExprBinaryOp(NodeExprBinaryOp binaryOp);
    public void VisitExprUnaryOp(NodeExprUnaryOp unaryOp);
    public void VisitExprFuncCall(NodeExprFuncCall funcCall);
    public void VisitExprNameRef(NodeExprNameRef nameRef);
    public void VisitExprBoolean(NodeExprBoolean boolean);
    public void VisitExprDecimal(NodeExprDecimal decimalExpr);
    public void VisitExprInt(NodeExprInt intExpr);
    public void VisitExprStr(NodeExprStr intExpr);
    
    // Statements
    public void VisitStatAssignment(NodeStatAssignment assignExpr);
    public void VisitStatBlock(NodeStatBlock block);
    public void VisitStatFuncCall(NodeStatFuncCall funcCall);
    public void VisitStatVarDecl(NodeStatVarDecl varDecl);
    public void VisitStatNoOperation();
}