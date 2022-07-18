using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a set of statements.
/// </summary>
public sealed class BlockAst : Ast
{
	/// <summary>
	/// Collection of executable statements.
	/// </summary>
	public readonly ImmutableArray<Ast> Statements;

	public BlockAst( TokenLocation location, ImmutableArray<Ast> statements ) : base( location )
	{
		Statements = statements;
	}
}
