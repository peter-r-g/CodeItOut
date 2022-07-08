using System.Collections.Generic;
using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid;

public partial class GridEntity : Entity
{
	[Net] public IntVector2 CellSize { get; set; }
	[Net] public IntVector2 Size { get; set; }
	[Net] public IntVector2 StartPosition { get; set; }
	
	[Net] public GridTraverser Traverser { get; private set; }
	[Net] public IList<GridCell> CellData { get; private set; }
	[Net] public IDictionary<IntVector2, TraverserItem> Items { get; private set; }

	public Vector2 WorldCenter => new(Position.x + WorldWidth / 2, Position.y + WorldHeight / 2);
	public float WorldWidth => Size.X * CellSize.X;
	public float WorldHeight => Size.Y * CellSize.Y;

	public override void Spawn()
	{
		base.Spawn();
		
		Traverser = new GridTraverser {Grid = this, Owner = this};
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
	
	public bool IsValidPosition( int x, int y )
	{
		if ( x < 0 || y < 0 )
			return false;

		if ( x > Size.X - 1 || y > Size.Y - 1 )
			return false;

		return true;
	}

	public void AddItemTo( int x, int y, TraverserItem item )
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
}
