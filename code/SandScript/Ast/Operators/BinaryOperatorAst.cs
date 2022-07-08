namespace SandScript.AbstractSyntaxTrees;

public sealed class BinaryOperatorAst : Ast
{
	public readonly Ast LeftAst;
	public readonly Token Operator;
	public readonly Ast RightAst;

	public TokenType OperatorType => Operator.Type;

	public BinaryOperatorAst( Ast leftAst, Token op, Ast rightAst ) : base( leftAst.StartLocation )
	{
		LeftAst = leftAst;
		Operator = op;
		RightAst = rightAst;
	}
}
