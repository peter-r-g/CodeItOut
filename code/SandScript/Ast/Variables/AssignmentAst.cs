namespace SandScript.AbstractSyntaxTrees;

public sealed class AssignmentAst : Ast
{
	public readonly VariableAst VariableAst;
	public readonly Token Operator;
	public readonly Ast ExpressionAst;

	public string VariableName => VariableAst.VariableName;
	public TokenType OperatorType => Operator.Type;

	public AssignmentAst( VariableAst variableAst, Token op, Ast expressionAst ) : base( variableAst.Token.Location )
	{
		VariableAst = variableAst;
		Operator = op;
		ExpressionAst = expressionAst;
	}
}
