using System;
using SandScript.AbstractSyntaxTrees;

namespace SandScript.Exceptions;

public sealed class NoVisitHandlerException : Exception
{
	public readonly Type AstType;

	public NoVisitHandlerException( Ast ast ) : base( "No visit handler for the " + ast.GetType() + " node" )
	{
		AstType = ast.GetType();
	}
}
