namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a unary operation.
/// </summary>
public sealed class UnaryOperatorAst : Ast
{
	/// <summary>
	/// The operator to be used on the operand.
	/// </summary>
	public readonly Token Operator;
	/// <summary>
	/// The expression to evaluate.
	/// </summary>
	public readonly Ast OperandAst;

	/// <summary>
	/// Helper property to quickly retrieve the operator type in <see cref="Operator"/>.
	/// </summary>
	public TokenType OperatorType => Operator.Type;

	public UnaryOperatorAst( Token op, Ast operandAst ) : base( op.Location )
	{
		Operator = op;
		OperandAst = operandAst;
	}
}
