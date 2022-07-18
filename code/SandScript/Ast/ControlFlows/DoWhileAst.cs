namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a do while looped statement
/// </summary>
public sealed class DoWhileAst : Ast
{
	/// <summary>
	/// The boolean expression to be evaluated for looping.
	/// </summary>
	public readonly Ast BooleanExpressionAst;
	/// <summary>
	/// The body to execute.
	/// </summary>
	public readonly BlockAst BodyAst;

	public DoWhileAst( TokenLocation location, Ast booleanExpressionAst, BlockAst bodyAst ) : base( location )
	{
		BooleanExpressionAst = booleanExpressionAst;
		BodyAst = bodyAst;
	}
}
