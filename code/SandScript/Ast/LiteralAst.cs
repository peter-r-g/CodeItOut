namespace SandScript.AbstractSyntaxTrees;

public class LiteralAst : Ast
{
	public readonly Token Token;
	public readonly ITypeProvider TypeProvider;

	public object Value => Token.Value;

	public LiteralAst( Token token, ITypeProvider typeProvider ) : base ( token.Location )
	{
		Token = token;
		TypeProvider = typeProvider;
	}
}
