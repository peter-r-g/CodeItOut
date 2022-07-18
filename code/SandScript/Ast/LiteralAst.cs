namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a literal value.
/// </summary>
public class LiteralAst : Ast
{
	/// <summary>
	/// The token that is holding the literal value.
	/// </summary>
	public readonly Token Token;
	/// <summary>
	/// The type that represents the literal value.
	/// </summary>
	public readonly ITypeProvider TypeProvider;

	/// <summary>
	/// Helper property to quickly retrieve the value this literal is holding.
	/// </summary>
	public object Value => Token.Value;

	public LiteralAst( Token token, ITypeProvider typeProvider ) : base ( token.Location )
	{
		Token = token;
		TypeProvider = typeProvider;
	}
}
