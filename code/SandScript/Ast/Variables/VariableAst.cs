namespace SandScript.AbstractSyntaxTrees;

public sealed class VariableAst : Ast
{
	public readonly Token Token;

	public string VariableName => (string)Token.Value;

	public VariableAst( Token token ) : base( token.Location )
	{
		Token = token;
	}
}
