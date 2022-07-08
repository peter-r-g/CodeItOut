namespace SandScript.AbstractSyntaxTrees;

public sealed class DoWhileAst : Ast
{
	public readonly Ast BooleanExpressionAst;
	public readonly BlockAst BodyAst;

	public DoWhileAst( TokenLocation location, Ast booleanExpressionAst, BlockAst bodyAst ) : base( location )
	{
		BooleanExpressionAst = booleanExpressionAst;
		BodyAst = bodyAst;
	}
}
