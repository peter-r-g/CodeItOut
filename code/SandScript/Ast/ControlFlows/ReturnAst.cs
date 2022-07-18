namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a return statements
/// </summary>
public sealed class ReturnAst : Ast
{
	/// <summary>
	/// The expression to be returned.
	/// </summary>
	public readonly Ast ExpressionAst;

	public ReturnAst( TokenLocation location, Ast expressionAst ) : base( location )
	{
		ExpressionAst = expressionAst;
	}
}
