using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SandScript;

internal class TypeCheckStack
{
	private readonly Stack<ITypeProvider> _typeStack = new();

	public void Push( ITypeProvider type )
	{
		_typeStack.Push( type );
	}

	public void Pop()
	{
		_typeStack.Pop();
	}
	
	public bool AssertTypeCheckLoose( ITypeProvider type, [NotNullWhen(false)] out ITypeProvider? expectedType )
	{
		if ( !_typeStack.TryPeek( out expectedType ) )
			return true;

		return type == expectedType || type == TypeProviders.Builtin.Variable ||
		       expectedType == TypeProviders.Builtin.Variable;
	}
}
