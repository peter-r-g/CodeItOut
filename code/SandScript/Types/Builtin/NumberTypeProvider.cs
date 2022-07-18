using System;
using System.Collections.Generic;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public sealed class NumberTypeProvider : ILiteralTypeProvider
{
	private const double NumberPrecisionTolerance = 0.001;

	public string TypeName => "Number";
	public string TypeIdentifier => "number";
	
	public Type BackingType => typeof(double);

	public Dictionary<TokenType, Func<object?, object?, object?>> BinaryOperations { get; } = new()
	{
		{TokenType.Plus, BinAdd},
		{TokenType.Minus, BinSub},
		{TokenType.Asterisk, BinMul},
		{TokenType.Slash, BinDiv},
		{TokenType.Percent, BinMod},
		{TokenType.Hat, BinPow},
		
		{TokenType.EqualsEquals, BinEquals},
		{TokenType.BangEquals, BinNotEquals},
		{TokenType.GreaterThan, BinGreaterThan},
		{TokenType.GreaterThanEquals, BinGreaterThanEquals},
		{TokenType.LessThan, BinLessThan},
		{TokenType.LessThanEquals, BinLessThanEquals}
	};

	public Dictionary<TokenType, Func<object?, object?>> UnaryOperations { get; } = new()
	{
		{TokenType.Plus, UnAdd}, {TokenType.Minus, UnSub}
	};

	public bool Compare( object? left, object? right )
	{
		return Math.Abs( (double)left! - (double)right! ) < NumberPrecisionTolerance;
	}

	public object CreateDefault()
	{
		return default(double);
	}

	public object? GetLiteral( Lexer lexer )
	{
		if ( !char.IsNumber( lexer.CurrentChar ) )
			return null;
		
		var startPos = lexer.Position;
		while ( !lexer.IsCurrentEof && char.IsNumber( lexer.CurrentChar ) )
			lexer.Advance();

		if ( lexer.CurrentChar != '.' )
			return double.Parse( lexer.Text.Substring( startPos, lexer.Position - startPos ) );
	    
		lexer.Advance();
	    
		while ( !lexer.IsCurrentEof && char.IsNumber( lexer.CurrentChar ) )
			lexer.Advance();

		return double.Parse( lexer.Text.Substring( startPos, lexer.Position - startPos ) );
	}

	public LiteralAst? GetLiteralAst( Token token )
	{
		return token.Value is double ? new LiteralAst( token, this ) : null;
	}

	public override string ToString()
	{
		return TypeName;
	}

	private static object? BinAdd( object? left, object? right )
	{
		return (double)left! + (double)right!;
	}

	private static object? BinSub( object? left, object? right )
	{
		return (double)left! - (double)right!;
	}

	private static object? BinMul( object? left, object? right )
	{
		return (double)left! * (double)right!;
	}

	private static object? BinDiv( object? left, object? right )
	{
		return (double)left! / (double)right!;
	}

	private static object? BinMod( object? left, object? right )
	{
		return (double)left! % (double)right!;
	}

	private static object BinPow( object? left, object? right )
	{
		return Math.Pow( (double)left!, (double)right! );
	}

	private static object? BinEquals( object? left, object? right )
	{
		return Math.Abs( (double)left! - (double)right! ) < NumberPrecisionTolerance;
	}

	private static object? BinNotEquals( object? left, object? right )
	{
		return !(bool)BinEquals( left, right )!;
	}

	private static object? BinGreaterThan( object? left, object? right )
	{
		return (double)left! > (double)right!;
	}

	private static object? BinGreaterThanEquals( object? left, object? right )
	{
		return (double)left! >= (double)right!;
	}

	private static object? BinLessThan( object? left, object? right )
	{
		return (double)left! < (double)right!;
	}

	private static object? BinLessThanEquals( object? left, object? right )
	{
		return (double)left! <= (double)right!;
	}

	private static object? UnAdd( object? operand )
	{
		return +(double)operand!;
	}

	private static object? UnSub( object? operand )
	{
		return -(double)operand!;
	}
}
