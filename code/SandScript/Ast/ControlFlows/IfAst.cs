namespace SandScript.AbstractSyntaxTrees;

public sealed class IfAst : Ast
{
	public readonly Ast BooleanExpressionAst;
	public readonly BlockAst TrueBodyAst;
	public readonly Ast FalseBodyAst;

	public IfAst( TokenLocation location, Ast booleanExpressionAst, BlockAst trueBodyAst, Ast falseBodyAst ) : base( location )
	{
		BooleanExpressionAst = booleanExpressionAst;
		TrueBodyAst = trueBodyAst;
		FalseBodyAst = falseBodyAst;
	}
}
