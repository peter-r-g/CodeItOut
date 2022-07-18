using System;

namespace SandScript.Exceptions;

public sealed class TypeMismatchException : Exception
{
	public readonly ITypeProvider ExpectedType;
	public readonly ITypeProvider? GotType;

	public TypeMismatchException( ITypeProvider expectedType, ITypeProvider? gotType )
		: base( "Expected \"" + expectedType.TypeName + "\", got \"" + gotType?.TypeName + "\"" )
	{
		ExpectedType = expectedType;
		GotType = gotType;
	}
}
