using System;
using System.Collections.Generic;
using System.Linq;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid;

public partial class GridCell : BaseNetworkable
{
	[Net] public GridMap GridMap { get; set; }
	[Net] public IntVector2 GridPosition { get; set; }
	[Net] public IDictionary<Direction, bool> CanMove { get; set; }
	[Net] public GridItem? GroundItem { get; set; }
	[Net] public IList<GridObject> Objects { get; set; }

	public Vector3 WorldPosition => GridMap.Position + new Vector3( GridMap.CellSize.X * GridPosition.X, GridMap.CellSize.Y * GridPosition.Y, 0 );
	public Vector3 WorldPositionCenter => WorldPosition + new Vector3( (float)GridMap.CellSize.X / 2, (float)GridMap.CellSize.Y / 2, 0 );
	
	public GridCell()
	{
		if ( !Host.IsServer )
			return;

		CanMove.Add( Direction.Up, false );
		CanMove.Add( Direction.Right, false );
		CanMove.Add( Direction.Down, false );
		CanMove.Add( Direction.Left, false );
	}

	public void Cleanup()
	{
		foreach ( var obj in Objects )
			obj.Delete();
	}

	public void AddObject( Direction dir, GridObject obj )
	{
		Host.AssertServer();
		
		if ( obj.IsObstructing() )
		{
			CanMove[dir] = false;
			if ( GridMap.TryGetCellInDirection( GridPosition.X, GridPosition.Y, dir, out var neighbourCell ) )
				neighbourCell.CanMove[dir.Opposite()] = false;
		}

		Objects.Add( obj );
		obj.GridMap = GridMap;
		obj.Cell = this;
		obj.Direction = dir;
		obj.Position = GetObjectPosition( dir ) + obj.Offset;
	}

	public IEnumerable<GridObject> GetObjectsInDirection( Direction dir )
	{
		return Objects.Where( obj => obj.Direction == dir );
	}

	public void Reset()
	{
		foreach ( var obj in Objects )
			obj.Reset();
	}

	private Vector3 GetObjectPosition( Direction dir )
	{
		var bl = WorldPosition;
		var br = bl + new Vector3( GridMap.CellSize.X, 0, 0 );
		var tl = bl + new Vector3( 0, GridMap.CellSize.Y, 0 );
		var tr = bl + new Vector3( GridMap.CellSize.X, GridMap.CellSize.Y, 0 );

		return dir switch
		{
			Direction.Up => Vector3.Lerp( tl, tr, 0.5f ),
			Direction.Right => Vector3.Lerp( tr, br, 0.5f ),
			Direction.Down => Vector3.Lerp( bl, br, 0.5f ),
			Direction.Left => Vector3.Lerp( bl, tl, 0.5f ),
			Direction.None => WorldPositionCenter,
			_ => throw new ArgumentOutOfRangeException( nameof(dir), dir, null )
		};
	}

	public void DebugDraw()
	{
		var bl = WorldPosition;
		var br = bl + new Vector3( GridMap.CellSize.X, 0, 0 );
		var tl = bl + new Vector3( 0, GridMap.CellSize.Y, 0 );
		var tr = bl + new Vector3( GridMap.CellSize.X, GridMap.CellSize.Y, 0 );
		
		DebugOverlay.Line( bl, br, CanMove[Direction.Down] ? Color.Green : Color.Red, 0.1f);
		DebugOverlay.Line( bl, tl, CanMove[Direction.Left] ? Color.Green : Color.Red, 0.1f);
		DebugOverlay.Line( tl, tr, CanMove[Direction.Up] ? Color.Green : Color.Red, 0.1f);
		DebugOverlay.Line( tr, br, CanMove[Direction.Right] ? Color.Green : Color.Red, 0.1f);
		
		DebugOverlay.Text( $"{GridPosition.X}, {GridPosition.Y}", WorldPositionCenter, Color.White, 0.1f );
		
		GroundItem?.DebugDraw();
		foreach ( var obj in Objects )
			obj.DebugDraw();
	}
}
