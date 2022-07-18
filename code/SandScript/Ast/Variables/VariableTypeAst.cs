namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a type.
/// </summary>
public sealed class VariableTypeAst : Ast
{
	/// <summary>
	/// The token that is holding the type.
	/// </summary>
	public readonly Token Token;

	/// <summary>
	/// The type held in the token.
	/// </summary>
	public readonly ITypeProvider TypeProvider;
	
	public VariableTypeAst( Token token ) : base( token.Location )
	{
		Token = token;
		TypeProvider = TypeProviders.GetByIdentifier( (string)token.Value )!;
	}
}
