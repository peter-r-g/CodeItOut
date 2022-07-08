using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

public sealed class ProgramAst : Ast
{
	public ImmutableArray<Ast> Statements { get; }

	public ProgramAst( ImmutableArray<Ast> statements ) : base( new TokenLocation( 0, 0 ) )
	{
		Statements = statements;
	}
}
