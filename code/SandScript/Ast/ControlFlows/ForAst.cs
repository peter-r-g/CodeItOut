namespace SandScript.AbstractSyntaxTrees;

public sealed class ForAst : Ast
{
	public readonly VariableDeclarationAst VariableDeclarationAst;
	public readonly Ast BooleanExpressionAst;
	public readonly AssignmentAst IteratorAst;
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
