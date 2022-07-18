namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a binary operation.
/// </summary>
public sealed class BinaryOperatorAst : Ast
{
	/// <summary>
	/// The left hand expression to evaluate.
	/// </summary>
	public readonly Ast LeftAst;
	/// <summary>
	/// The operator to be used with both operands.
	/// </summary>
	public readonly Token Operator;
	/// <summary>
	/// The right hand expression to evaluate.
	/// </summary>
	public readonly Ast RightAst;

	/// <summary>
	/// Helper property to quickly retrieve the operator type in <see cref="Operator"/>.
	/// </summary>
	public TokenType OperatorType => Operator.Type;

	public BinaryOperatorAst( Ast leftAst, Token op, Ast rightAst ) : base( leftAst.StartLocation )
	{
		LeftAst = leftAst;
		Operator = op;
		RightAst = rightAst;
	}
}
