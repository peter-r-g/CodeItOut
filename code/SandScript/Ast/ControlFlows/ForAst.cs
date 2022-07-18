namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a for loop statement.
/// </summary>
public sealed class ForAst : Ast
{
	/// <summary>
	/// The number declaration.
	/// </summary>
	public readonly VariableDeclarationAst VariableDeclarationAst;
	/// <summary>
	/// The boolean expression to be evaluated for looping.
	/// </summary>
	public readonly Ast BooleanExpressionAst;
	/// <summary>
	/// The assignment to be executed after every completion of the <see cref="BodyAst"/>.
	/// </summary>
	public readonly AssignmentAst IteratorAst;
	/// <summary>
	/// The body to execute.
	/// </summary>
	public readonly BlockAst BodyAst;

	public ForAst( TokenLocation location, VariableDeclarationAst variableDeclarationAst, Ast booleanExpressionAst,
		AssignmentAst iteratorAst, BlockAst bodyAst ) : base( location )
	{
		VariableDeclarationAst = variableDeclarationAst;
		BooleanExpressionAst = booleanExpressionAst;
		IteratorAst = iteratorAst;
		BodyAst = bodyAst;
	}
}
