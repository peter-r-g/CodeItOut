using System;

namespace CodeItOut.Utility;

public static class DirectionExtension
{
	public static Direction Opposite( this Direction dir )
	{
		return dir switch
		{
			Direction.Up => Direction.Down,
			Direction.Right => Direction.Left,
			Direction.Down => Direction.Up,
			Direction.Left => Direction.Right,
			_ => throw new ArgumentOutOfRangeException( nameof(dir), dir, null )
		};
	}

	public static IntVector2 ToPosition( this Direction dir )
	{
		return dir switch
		{
			Direction.Up => IntVector2.Up,
			Direction.Right => IntVector2.Right,
			Direction.Down => IntVector2.Down,
			Direction.Left => IntVector2.Left,
			_ => throw new ArgumentOutOfRangeException( nameof(dir), dir, null )
		};
	}
}
