using System;
using System.Collections.Generic;

namespace SandScript;

public interface ITypeProvider
{
	string TypeName { get; }
	string TypeIdentifier { get; }
	
	Type BackingType { get; }
	
	Dictionary<TokenType, Func<object?, object?, object?>> BinaryOperations { get; }
	Dictionary<TokenType, Func<object?, object?>> UnaryOperations { get; }

	bool Compare( object? left, object? right );
	object? CreateDefault();
}
