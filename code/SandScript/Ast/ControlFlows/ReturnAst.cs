namespace SandScript.AbstractSyntaxTrees;

public sealed class ReturnAst : Ast
{
	public readonly Ast ExpressionAst;

	public ReturnAst( TokenLocation location, Ast expressionAst ) : base( location )
	{
		ExpressionAst = expressionAst;
	}
}
