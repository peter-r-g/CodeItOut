namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents an if statement.
/// </summary>
public sealed class IfAst : Ast
{
	/// <summary>
	/// The boolean expression to be evaluated.
	/// </summary>
	public readonly Ast BooleanExpressionAst;
	/// <summary>
	/// The body to execute if <see cref="BooleanExpressionAst"/> is true.
	/// </summary>
	public readonly BlockAst TrueBodyAst;
	/// <summary>
	/// The body to execute if <see cref="BooleanExpressionAst"/> is false.
	/// NOTE: This can be a <see cref="NoOperationAst"/> in cases where no "else" is used.
	/// </summary>
	public readonly Ast FalseBodyAst;

	public IfAst( TokenLocation location, Ast booleanExpressionAst, BlockAst trueBodyAst, Ast falseBodyAst ) : base( location )
	{
		BooleanExpressionAst = booleanExpressionAst;
		TrueBodyAst = trueBodyAst;
		FalseBodyAst = falseBodyAst;
	}
}
