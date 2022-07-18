namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents an assignment statement.
/// </summary>
public sealed class AssignmentAst : Ast
{
	/// <summary>
	/// The variable name to be assigned.
	/// </summary>
	public readonly VariableAst VariableAst;
	/// <summary>
	/// The assignment operator used.
	/// </summary>
	public readonly Token Operator;
	/// <summary>
	/// The expression to apply to the variable.
	/// </summary>
	public readonly Ast ExpressionAst;

	/// <summary>
	/// Helper property to quickly retrieve the name of the variable being assigned.
	/// </summary>
	public string VariableName => VariableAst.VariableName;
	/// <summary>
	/// Helper property to quickly retrieve the type of assignment operator.
	/// </summary>
	public TokenType OperatorType => Operator.Type;

	public AssignmentAst( VariableAst variableAst, Token op, Ast expressionAst ) : base( variableAst.Token.Location )
	{
		VariableAst = variableAst;
		Operator = op;
		ExpressionAst = expressionAst;
	}
}
