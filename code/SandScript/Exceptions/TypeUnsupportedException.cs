using System;

namespace SandScript.Exceptions;

public sealed class TypeUnsupportedException : Exception
{
	public readonly Type? UnsupportedType;
	public readonly string UnsupportedTypeIdentifier;

	public TypeUnsupportedException( Type type ) : base( "The type \"" + type + "\" is unsupported" )
	{
		UnsupportedType = type;
		UnsupportedTypeIdentifier = type.Name;
	}

	public TypeUnsupportedException( string identifier ) : base( "The type \"" + identifier + "\" is unsupported" )
	{
		UnsupportedType = null;
		UnsupportedTypeIdentifier = identifier;
	}
}
