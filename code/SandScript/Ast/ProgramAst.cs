using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Functionally equivalent to <see cref="BlockAst"/> but used to represent the entire parsed program.
/// </summary>
public sealed class ProgramAst : Ast
{
	/// <summary>
	/// Collection of executable statements. See <see cref="BlockAst"/>
	/// </summary>
	public ImmutableArray<Ast> Statements { get; }

	public ProgramAst( ImmutableArray<Ast> statements ) : base( new TokenLocation( 0, 0 ) )
	{
		Statements = statements;
	}
}
