namespace SandScript.AbstractSyntaxTrees;

public sealed class UnaryOperatorAst : Ast
{
	public readonly Token Operator;
	public readonly Ast OperandAst;

	public TokenType OperatorType => Operator.Type;

	public UnaryOperatorAst( Token op, Ast operandAst ) : base( op.Location )
	{
		Operator = op;
		OperandAst = operandAst;
	}
}
