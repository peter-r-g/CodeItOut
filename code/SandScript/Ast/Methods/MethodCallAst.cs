using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

public sealed class MethodCallAst : Ast
{
	public readonly Token NameToken;
	public readonly ImmutableArray<Ast> ArgumentAsts;
	internal ImmutableArray<ITypeProvider> ArgumentTypes;

	public string MethodName => (string)NameToken.Value;

	public MethodCallAst( Token nameToken, ImmutableArray<Ast> argumentAsts )
		: this( nameToken, argumentAsts, ImmutableArray<ITypeProvider>.Empty )
	{
	}

	internal MethodCallAst( Token nameToken, ImmutableArray<Ast> argumentAsts, ImmutableArray<ITypeProvider> argumentTypes )
		: base( nameToken.Location )
	{
		NameToken = nameToken;
		ArgumentAsts = argumentAsts;
		ArgumentTypes = argumentTypes;
	}
}
