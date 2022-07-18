namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a while statement.
/// </summary>
public sealed class WhileAst : Ast
{
	/// <summary>
	/// The boolean expression to be evaluated for looping.
	/// </summary>
	public readonly Ast BooleanExpressionAst;
	/// <summary>
	/// The body to execute.
	/// </summary>
	public readonly BlockAst BodyAst;

	public WhileAst( TokenLocation location, Ast booleanExpressionAst, BlockAst bodyAst ) : base( location )
	{
		BooleanExpressionAst = booleanExpressionAst;
		BodyAst = bodyAst;
	}
}
