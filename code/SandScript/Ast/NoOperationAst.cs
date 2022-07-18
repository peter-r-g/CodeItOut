namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents an empty operation.
/// </summary>
public sealed class NoOperationAst : Ast
{
	public NoOperationAst( TokenLocation location ) : base( location ) { }
}
