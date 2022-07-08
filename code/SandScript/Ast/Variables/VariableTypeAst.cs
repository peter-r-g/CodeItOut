namespace SandScript.AbstractSyntaxTrees;

public sealed class VariableTypeAst : Ast
{
	public readonly Token Token;

	public readonly ITypeProvider TypeProvider;
	
	public VariableTypeAst( Token token ) : base( token.Location )
	{
		Token = token;
		TypeProvider = TypeProviders.GetByIdentifier( (string)token.Value )!;
	}
}
