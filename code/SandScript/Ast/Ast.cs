using System;

namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// The base class for all abstract syntax tree nodes.
/// </summary>
public abstract class Ast
{
	/// <summary>
	/// The starting location of the node.
	/// </summary>
	public readonly TokenLocation StartLocation;
	/// <summary>
	/// Every Ast node requires a Guid as they can be used in containers to hold data relating to the Ast.
	/// </summary>
	public readonly Guid Guid = Guid.NewGuid();

	protected Ast( TokenLocation startLocation )
	{
		StartLocation = startLocation;
	}
}
