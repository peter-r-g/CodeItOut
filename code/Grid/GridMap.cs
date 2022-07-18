﻿using System;
using System.Collections.Generic;
using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;
using SandScript;

namespace CodeItOut.Grid;

public partial class GridMap : Entity
{
	[Net] public IntVector2 CellSize { get; set; }
	[Net] public IntVector2 Size { get; set; }
	[Net] public IntVector2 StartPosition { get; set; }
	
	[Net] public GridTraverser Traverser { get; private set; }
	[Net] public IList<GridCell> CellData { get; private set; }
	[Net] public IDictionary<IntVector2, GridItem> Items { get; private set; }
	[Net] public IList<GridEntity> Entities { get; private set; }

	public Vector2 WorldCenter => new(Position.x + WorldWidth / 2, Position.y + WorldHeight / 2);
	public float WorldWidth => Size.X * CellSize.X;
	public float WorldHeight => Size.Y * CellSize.Y;

	public override void Spawn()
	{
		base.Spawn();

		Traverser = new GridTraverser {GridMap = this, Owner = this};
	}
	
	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		
		for ( var x = 0; x < Size.X; x++ )
		{
			for ( var y = 0; y < Size.Y; y++ )
			{
				var cell = CellData[GetIndexAt( x, y )];
				cell.DebugDraw();
			}
		}
	}

	public async void Run()
	{
		for ( var i = 0; i < Traverser.ActionCount; i++ )
		{
			var result = await Traverser.RunAction( i );
			if ( result == ActionState.Failed )
			{
				Log.Info( "Failed" );
				await GameTask.DelaySeconds( 5 );
				Reset();
				return;
			}

			foreach ( var entity in Entities )
				await entity.RunAction( i );
		}

		Log.Info( "Completed" );
		await GameTask.DelaySeconds( 5 );
		Reset();
	}

	public void Reset()
	{
		Host.AssertServer();
		
		Traverser.Reset();

		foreach ( var (startPosition, item) in Items )
		{
			if ( !TryGetCellAt( startPosition.X, startPosition.Y, out var cellInfo ) )
			{
				Log.Error( "Failed to get items original position." );
				continue;
			}
			
			item.OnDrop( cellInfo );
		}

		foreach ( var cell in CellData )
			cell.Reset();
	}

	public bool IsValidPosition( int x, int y )
	{
		if ( x < 0 || y < 0 )
			return false;

		return x <= Size.X - 1 && y <= Size.Y - 1;
	}

	public void AddItemTo( int x, int y, GridItem item )
	{
		if ( !TryGetCellAt( x, y, out var cellInfo ) )
		{
			Log.Error( $"Failed to add item to {x}, {y} because the X, Y provided is not valid." );
			return;
		}

		if ( cellInfo.GroundItem is not null )
		{
			Log.Error( $"Failed to add item to {x}, {y} because an item is already there." );
		}

		Items.Add( new IntVector2( x, y ), item );
		item.OnDrop( cellInfo );
	}

	public void PlaceObjectAt( int x, int y, Direction objDirection, GridObject obj )
	{
		if ( !TryGetCellAt( x, y, out var cellInfo ) )
		{
			Log.Error( $"Failed to place object to {{x}}, {y} because the X, Y provided is not valid." );
			return;
		}

		cellInfo.AddObject( objDirection, obj );
	}

	public void PlaceEntityAt( int x, int y, GridEntity entity )
	{
		if ( !TryGetCellAt( x, y, out var cellInfo ) )
		{
			Log.Error( $"Failed to place entity to {{x}}, {y} because the X, Y provided is not valid." );
			return;
		}

		entity.GridPosition = new IntVector2( x, y );
		Entities.Add( entity );
	}

	public bool TryGetCellAt( int x, int y, out GridCell gridCell )
	{
		if ( !IsValidPosition( x, y ) )
		{
			gridCell = null;
			return false;
		}

		gridCell = CellData[GetIndexAt( x, y )];
		return true;
	}

	public bool TryGetCellInDirection( int x, int y, Direction dir, out GridCell gridCell )
	{
		if ( !IsValidPosition( x, y ) )
		{
			gridCell = null;
			return false;
		}

		var targetCellPosition = new IntVector2( x, y ) + dir.ToPosition();
		return TryGetCellAt( targetCellPosition.X, targetCellPosition.Y, out gridCell );
	}

	private int GetIndexAt( int x, int y )
	{
		return x + Size.X * y;
	}

	public static GridMap Load( BaseFileSystem fs, string filePath )
	{
		var mapScript = new Script();
		mapScript.AddClassMethods<SandScriptInterop.MapMaking>();
		mapScript.Execute( fs.ReadAllText( filePath ) );
		var map = SandScriptInterop.MapMaking.MapBuilder.Build();
		SandScriptInterop.MapMaking.MapBuilder = new GridBuilder();

		return map;
	}
}