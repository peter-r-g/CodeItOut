using System;
using System.Collections.Generic;

namespace SandScript;

public sealed class MethodTypeProvider : ITypeProvider
{
	public string TypeName => "Method";
	public string TypeIdentifier => string.Empty;
	
	public Type BackingType => typeof(ScriptMethod);
	
	public Dictionary<TokenType, Func<object?, object?, object?>> BinaryOperations { get; } = new();
	public Dictionary<TokenType, Func<object?, object?>> UnaryOperations { get; } = new();

	public bool Compare( object? left, object? right )
	{
		return ((ScriptMethod)left!).Signature.Equals( ((ScriptMethod)right!).Signature );
	}

	public object? CreateDefault()
	{
		return null;
	}

	public override string ToString()
	{
		return TypeName;
	}
}
