using System;

namespace SandScript;

public sealed class TokenLocation : IEquatable<TokenLocation>
{
	public static readonly TokenLocation Zero = new(0, 0);
	
	public int Row { get; }
	public int Column { get; }

	public TokenLocation( int row, int column )
	{
		Row = row;
		Column = column;
	}

	public override string ToString()
	{
		return Row + ":" + Column;
	}

	public bool Equals( TokenLocation? other )
	{
		if ( other is null )
			return false;
		
		return Row == other.Row && Column == other.Column;
	}

	public override bool Equals( object? obj )
	{
		return obj is TokenLocation other && Equals( other );
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( Row, Column );
	}

	public static bool operator ==( TokenLocation left, TokenLocation right )
	{
		return left.Row == right.Row && left.Column == right.Column;
	}

	public static bool operator !=( TokenLocation left, TokenLocation right )
	{
		return !(left == right);
	}
}
