using System;

namespace SandScript.Exceptions;

public sealed class ReturnTypeUnsupportedException : Exception
{
	public readonly Type UnsupportedType;

	public ReturnTypeUnsupportedException( Type type ) : base( "The return type " + type.Name + " is not supported" )
	{
		UnsupportedType = type;
	}
}
