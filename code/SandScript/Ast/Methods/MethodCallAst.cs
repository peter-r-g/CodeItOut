using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a method call statement.
/// </summary>
public sealed class MethodCallAst : Ast
{
	/// <summary>
	/// The token that contains the name of the method.
	/// </summary>
	public readonly Token NameToken;
	/// <summary>
	/// The list of arguments to be passed.
	/// </summary>
	public readonly ImmutableArray<Ast> ArgumentAsts;
	/// <summary>
	/// For internal use. This is populated by the <see cref="SemanticAnalyzer"/> to help define the types used in <see cref="ArgumentAsts"/>.
	/// </summary>
	internal ImmutableArray<ITypeProvider> ArgumentTypes;

	/// <summary>
	/// Helper property to quickly retrieve the name of the method being called.
	/// </summary>
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
