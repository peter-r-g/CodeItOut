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
			Direction.None => Direction.None,
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
			Direction.None => IntVector2.Zero,
			_ => throw new ArgumentOutOfRangeException( nameof(dir), dir, null )
		};
	}

	public static Rotation ToRotation( this Direction dir )
	{
		return dir switch
		{
			Direction.Up => Rotation.From( 0, 90, 0 ),
			Direction.Right => Rotation.From( 0, 0, 0 ),
			Direction.Down => Rotation.From( 0, 270, 0 ),
			Direction.Left => Rotation.From( 0, 180, 0 ),
			Direction.None => Rotation.From( 0, 0, 0 ),
			_ => throw new ArgumentOutOfRangeException( nameof(dir), dir, null )
		};
	}
}
