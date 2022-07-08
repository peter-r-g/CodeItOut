using System;

namespace SandScript.AbstractSyntaxTrees;

public abstract class Ast
{
	public readonly TokenLocation StartLocation;
	public readonly Guid Guid = Guid.NewGuid();

	protected Ast( TokenLocation startLocation )
	{
		StartLocation = startLocation;
	}
}
