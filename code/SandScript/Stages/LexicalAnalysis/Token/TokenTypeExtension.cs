using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SandScript;

public static class TokenTypeExtension
{
	private static readonly Dictionary<string, TokenType> Keywords = new()
	{
		{"do", TokenType.Do},
		{"else", TokenType.Else},
		{"for", TokenType.For},
		{"if", TokenType.If},
		{"return", TokenType.Return},
		{"while", TokenType.While}
	};

	private static readonly Dictionary<string, TokenType> GeneralTokens = new()
	{
		{"&&", TokenType.AmpersandAmpersand},
		{"*", TokenType.Asterisk},
		{"*=", TokenType.AsteriskEquals},
		{"!", TokenType.Bang},
		{"!=", TokenType.BangEquals},
		{",", TokenType.Comma},
		{"=", TokenType.Equals},
		{"==", TokenType.EqualsEquals},
		{">", TokenType.GreaterThan},
		{">=", TokenType.GreaterThanEquals},
		{"^", TokenType.Hat},
		{"{", TokenType.LeftCurlyBracket},
		{"(", TokenType.LeftParenthesis},
		{"[", TokenType.LeftSquareBracket},
		{"<", TokenType.LessThan},
		{"<=", TokenType.LessThanEquals},
		{"-", TokenType.Minus},
		{"-=", TokenType.MinusEquals},
		{"%", TokenType.Percent},
		{"%=", TokenType.PercentEquals},
		{".", TokenType.Period},
		{"||", TokenType.PipePipe},
		{"+", TokenType.Plus},
		{"+=", TokenType.PlusEquals},
		{")", TokenType.RightParenthesis},
		{"}", TokenType.RightCurlyBracket},
		{"]", TokenType.RightSquareBracket},
		{";", TokenType.SemiColon},
		{"/", TokenType.Slash},
		{"/=", TokenType.SlashEquals}
	};

	public static bool TryParseKeyword( string str, [NotNullWhen(true)] out TokenType? keywordType )
	{
		if ( Keywords.TryGetValue( str, out var parsedType ) )
		{
			keywordType = parsedType;
			return true;
		}

		keywordType = null;
		return false;
	}

	public static bool TryParseToken( string str, [NotNullWhen( true )] out TokenType? tokenType )
	{
		if ( GeneralTokens.TryGetValue( str, out var parsedType ) )
		{
			tokenType = parsedType;
			return true;
		}

		tokenType = null;
		return false;
	}

	public static string GetString( this TokenType tokenType )
	{
		if ( IsKeyword( tokenType ) )
		{
			foreach ( var keyword in Keywords )
			{
				if ( keyword.Value == tokenType )
					return keyword.Key;
			}
		}

		foreach ( var general in GeneralTokens )
		{
			if ( general.Value == tokenType )
				return general.Key;
		}

		throw new Exception();
	}

	public static int GetUnaryOperatorPrecedence( this TokenType tokenType )
	{
		return tokenType switch
		{
			TokenType.Plus => 2,
			TokenType.Minus => 2,
			TokenType.Bang => 2,
			_ => int.MaxValue
		};
	}

	public static int GetBinaryOperatorPrecedence( this TokenType tokenType )
	{
		return tokenType switch
		{
			TokenType.Period => 1,
			
			TokenType.Asterisk => 3,
			TokenType.Slash => 3,
			TokenType.Percent => 3,
			
			TokenType.Plus => 4,
			TokenType.Minus => 4,
			
			TokenType.LessThan => 6,
			TokenType.LessThanEquals => 6,
			TokenType.GreaterThan => 6,
			TokenType.GreaterThanEquals => 6,
			
			TokenType.EqualsEquals => 7,
			TokenType.BangEquals => 7,

			TokenType.AmpersandAmpersand => 11,
			
			TokenType.PipePipe => 12,

			_ => int.MaxValue
		};
	}

	public static bool IsKeyword( this TokenType tokenType )
	{
		return tokenType switch
		{
			TokenType.Do => true,
			TokenType.Else => true,
			TokenType.For => true,
			TokenType.If => true,
			TokenType.Return => true,
			TokenType.While => true,
			_ => false
		};
	}

	public static bool IsAssignmentOperator( this TokenType tokenType )
	{
		return tokenType switch
		{
			TokenType.AsteriskEquals => true,
			TokenType.Equals => true,
			TokenType.MinusEquals => true,
			TokenType.PercentEquals => true,
			TokenType.PlusEquals => true,
			TokenType.SlashEquals => true,
			_ => false
		};
	}

	public static bool IsNonEssential( this TokenType tokenType )
	{
		return tokenType switch
		{
			TokenType.Comment => true,
			TokenType.MultiLineComment => true,
			TokenType.Whitespace => true,
			_ => false
		};
	}

	public static ITypeProvider GetOperatorResultType( this TokenType tokenType, ITypeProvider inputType )
	{
		return tokenType switch
		{
			TokenType.EqualsEquals => TypeProviders.Builtin.Boolean,
			TokenType.BangEquals => TypeProviders.Builtin.Boolean,
			TokenType.GreaterThan => TypeProviders.Builtin.Boolean,
			TokenType.GreaterThanEquals => TypeProviders.Builtin.Boolean,
			TokenType.LessThan => TypeProviders.Builtin.Boolean,
			TokenType.LessThanEquals => TypeProviders.Builtin.Boolean,
			TokenType.Bang => TypeProviders.Builtin.Boolean,
			_ => inputType
		};
	}

	public static TokenType GetBinaryOperatorOfAssignment( this TokenType tokenType )
	{
		return tokenType switch
		{
			TokenType.AsteriskEquals => TokenType.Asterisk,
			TokenType.MinusEquals => TokenType.Minus,
			TokenType.PercentEquals => TokenType.Percent,
			TokenType.PlusEquals => TokenType.Plus,
			TokenType.SlashEquals => TokenType.Slash,
			_ => TokenType.None
		};
	}
}
