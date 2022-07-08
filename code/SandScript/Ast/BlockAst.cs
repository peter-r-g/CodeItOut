using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

public sealed class BlockAst : Ast
{
	public readonly ImmutableArray<Ast> Statements;

	public BlockAst( TokenLocation location, ImmutableArray<Ast> statements ) : base( location )
	{
		Statements = statements;
	}
}
