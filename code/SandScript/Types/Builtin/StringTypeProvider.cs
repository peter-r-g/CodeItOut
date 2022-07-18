using System;
using System.Collections.Generic;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public sealed class StringTypeProvider : ILiteralTypeProvider
{
	public string TypeName => "String";
	public string TypeIdentifier => "string";
	
	public Type BackingType => typeof(string);

	public Dictionary<TokenType, Func<object?, object?, object?>> BinaryOperations { get; } = new()
	{
		{TokenType.Plus, BinAdd},
		
		{TokenType.EqualsEquals, BinEquals},
		{TokenType.BangEquals, BinNotEquals}
	};

	public Dictionary<TokenType, Func<object?, object?>> UnaryOperations { get; } = new();

	public bool Compare( object? left, object? right )
	{
		return (string)left! == (string)right!;
	}

	public object CreateDefault()
	{
		return string.Empty;
	}

	public object? GetLiteral( Lexer lexer )
	{
		if ( lexer.CurrentChar != '"' )
			return null;
		
		var location = new TokenLocation( lexer.Row, lexer.Column );
		
		lexer.Advance();
		var startPos = lexer.Position;
		while ( !lexer.IsCurrentEof && lexer.CurrentChar != '"' )
			lexer.Advance();
		var str = lexer.Text.Substring( startPos, lexer.Position - startPos );

		if ( lexer.CurrentChar != '"' )
			lexer.Diagnostics.UnclosedString( location );
		else
			lexer.Advance();

		return str;
	}

	public LiteralAst? GetLiteralAst( Token token )
	{
		return token.Value is string ? new LiteralAst( token, this ) : null;
	}

	public override string ToString()
	{
		return TypeName;
	}

	private static object BinAdd( object? left, object? right )
	{
		return (string)left! + (string)right!;
	}

	private static object? BinEquals( object? left, object? right )
	{
		return (string)left! == (string)right!;
	}

	private static object? BinNotEquals( object? left, object? right )
	{
		return !(bool)BinEquals( left, right )!;
	}
}
