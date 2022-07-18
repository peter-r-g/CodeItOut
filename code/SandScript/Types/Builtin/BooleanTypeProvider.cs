using System;
using System.Collections.Generic;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public sealed class BooleanTypeProvider : ILiteralTypeProvider
{
	public string TypeName => "Boolean";
	public string TypeIdentifier => "bool";

	public Type BackingType => typeof(bool);

	public Dictionary<TokenType, Func<object?, object?, object?>> BinaryOperations { get; } = new()
	{
		{TokenType.AmpersandAmpersand, And},
		{TokenType.PipePipe, Or},
		{TokenType.EqualsEquals, BinEquals},
		{TokenType.BangEquals, BinNotEquals}
	};

	public Dictionary<TokenType, Func<object?, object?>> UnaryOperations { get; } = new()
	{
		{TokenType.Bang, Flip}
	};

	public bool Compare( object? left, object? right )
	{
		return (bool)left! == (bool)right!;
	}

	public object CreateDefault()
	{
		return default(bool);
	}

	public object? GetLiteral( Lexer lexer )
	{
		switch (lexer.CurrentChar)
		{
			case 't':
				if ( lexer.Peek() != 'r' || lexer.Peek( 2 ) != 'u' || lexer.Peek( 3 ) != 'e' )
					return null;

				for ( var i = 0; i < 4; i++ )
					lexer.Advance();
				
				return true;
			case 'f':
				if ( lexer.Peek() != 'a' || lexer.Peek( 2 ) != 'l' || lexer.Peek( 3 ) != 's' || lexer.Peek( 4 ) != 'e' )
					return null;

				for ( var i = 0; i < 5; i++ )
					lexer.Advance();
				
				return false;
		}

		return null;
	}

	public LiteralAst? GetLiteralAst( Token token )
	{
		return token.Value is bool ? new LiteralAst( token, this ) : null;
	}

	public override string ToString()
	{
		return TypeName;
	}

	private static object And( object? left, object? right )
	{
		return (bool)left! && (bool)right!;
	}

	private static object Or( object? left, object? right )
	{
		return (bool)left! || (bool)right!;
	}

	private static object? Flip( object? operand )
	{
		return !(bool)operand!;
	}

	private static object? BinEquals( object? left, object? right )
	{
		return (bool)left! == (bool)right!;
	}

	private static object? BinNotEquals( object? left, object? right )
	{
		return !(bool)BinEquals( left, right )!;
	}
}
