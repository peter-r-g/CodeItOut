namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a variable.
/// </summary>
public sealed class VariableAst : Ast
{
	/// <summary>
	/// The token that holds the variable name.
	/// </summary>
	public readonly Token Token;

	/// <summary>
	/// Helper property to quickly retrieve the variable name.
	/// </summary>
	public string VariableName => (string)Token.Value;

	public VariableAst( Token token ) : base( token.Location )
	{
		Token = token;
	}
}
