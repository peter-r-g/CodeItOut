using System;

namespace SandScript.Exceptions;

public sealed class ParameterException : Exception
{
	public ParameterException( string message ) : base( message )
	{
	}
}
