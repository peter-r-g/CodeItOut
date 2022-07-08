namespace SandScript.AbstractSyntaxTrees;

public sealed class WhileAst : Ast
{
	public readonly Ast BooleanExpressionAst;
	public readonly BlockAst BodyAst;

	public WhileAst( TokenLocation location, Ast booleanExpressionAst, BlockAst bodyAst ) : base( location )
	{
		BooleanExpressionAst = booleanExpressionAst;
		BodyAst = bodyAst;
	}
}
