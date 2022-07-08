using System;

namespace SandScript.Exceptions;

public sealed class RuntimeException : Exception
{
	public RuntimeException( Exception innerException )
		: base( "An exception occurred at runtime.", innerException )
	{
	}
}
