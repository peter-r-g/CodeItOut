using System;

namespace CodeItOut.Utility;

public readonly struct IntVector2 : IEquatable<IntVector2>
{
	public static IntVector2 Zero => new(0, 0);
	public static IntVector2 One => new(1, 1);
	public static IntVector2 Up => new(0, 1);
	public static IntVector2 Right => new(1, 0);
	public static IntVector2 Down => new(0, -1);
	public static IntVector2 Left => new(-1, 0);
	
	public readonly int X;
	public readonly int Y;

	public IntVector2( int x, int y )
	{
		X = x;
		Y = y;
	}

	public IntVector2 Clamp( IntVector2 min, IntVector2 max )
	{
		return new IntVector2( Math.Clamp( X, min.X, max.X ), Math.Clamp( Y, min.Y, max.Y ) );
	}

	public IntVector2 WithX( int newX )
	{
		return new IntVector2( newX, Y );
	}

	public IntVector2 WithY( int newY )
	{
		return new IntVector2( X, newY );
	}
	
	public bool Equals( IntVector2 other )
	{
		return X == other.X && Y == other.Y;
	}

	public override bool Equals( object obj )
	{
		return obj is IntVector2 vector2 && Equals( vector2 );
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( X, Y );
	}
	
	public override string ToString()
	{
		return $"{X},{Y}";
	}

	public static implicit operator Vector2( IntVector2 vec ) => new(vec.X, vec.Y);

	public static IntVector2 operator +( IntVector2 v1, IntVector2 v2 ) => new(v1.X + v2.X, v1.Y + v2.Y);
	public static IntVector2 operator -( IntVector2 v1, IntVector2 v2 ) => new(v1.X - v2.X, v1.Y - v2.Y);
	public static IntVector2 operator *( IntVector2 vec, int i ) => new(vec.X * i, vec.Y * i);
	public static IntVector2 operator *( int i, IntVector2 vec ) => new(vec.X * i, vec.Y * i);
	public static IntVector2 operator /( IntVector2 vec, int i ) => new(vec.X / i, vec.Y / i);
	public static IntVector2 operator /( int i, IntVector2 vec ) => new(vec.X / i, vec.Y / i);
	
	public static bool operator ==( IntVector2 left, IntVector2 right ) => left.Equals( right );
	public static bool operator !=( IntVector2 left, IntVector2 right ) => !(left == right);
}
