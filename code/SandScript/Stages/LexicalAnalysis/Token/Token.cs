using System;

namespace SandScript;

public sealed class Token : IEquatable<Token>
{
	public TokenType Type { get; }
	public object Value { get; }
	public TokenLocation Location { get; }

	public Token( TokenType type, object value, int row, int column )
		: this( type, value, new TokenLocation( row, column ) )
	{
	}

	public Token( TokenType type, object value, TokenLocation location )
	{
		Type = type;
		Value = value;
		Location = location;
	}

	public bool Equals( Token? other )
	{
		if ( other is null )
			return false;
		
		return Type == other.Type && Value.Equals( other.Value ) && Location.Equals( other.Location );
	}

	public override bool Equals( object? obj )
	{
		return obj is Token other && Equals( other );
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( (int)Type, Value, Location );
	}

	public override string ToString()
	{
		return Type.ToString() + ':' + Value + " (" + Location + ')';
	}
}
