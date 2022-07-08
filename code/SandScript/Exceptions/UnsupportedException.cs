using System;

namespace SandScript.Exceptions;

public sealed class UnsupportedException : Exception
{
	public UnsupportedException() : base( "Operation is not supported" )
	{
	}
}
