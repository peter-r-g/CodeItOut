using System;
using System.Collections.Generic;
using CodeItOut.Items;
using CodeItOut.Utility;

namespace CodeItOut.Grid;

public class GridBuilder
{
	private IntVector2 _cellSize;
	private IntVector2 _size;
	private IntVector2 _startPosition;
	private int _itemCap;
	private Vector3 _worldPosition;

	private readonly Dictionary<(int, int), Type> _items = new();
	private readonly Dictionary<(int, int), List<(Type, Direction)>> _objects = new();
	
	public GridBuilder WithCellSize( IntVector2 cellSize )
	{
		_cellSize = cellSize;
		return this;
	}

	public GridBuilder WithCellSize( int x, int y )
	{
		_cellSize = new IntVector2( x, y );
		return this;
	}

	public GridBuilder WithSize( IntVector2 size )
	{
		_size = size;
		return this;
	}

	public GridBuilder WithSize( int x, int y )
	{
		_size = new IntVector2( x, y );
		return this;
	}

	public GridBuilder WithStartPosition( IntVector2 startPosition )
	{
		_startPosition = startPosition;
		return this;
	}

	public GridBuilder WithStartPosition( int x, int y )
	{
		_startPosition = new IntVector2( x, y );
		return this;
	}

	public GridBuilder WithItemCap( int itemCap )
	{
		_itemCap = itemCap;
		return this;
	}

	public GridBuilder WithWorldPosition( Vector3 position )
	{
		_worldPosition = position;
		return this;
	}
	
	public GridBuilder AddItem( int x, int y, Type gridItemType )
	{
		_items.Add( (x, y), gridItemType );
		return this;
	}

	public GridBuilder AddObject( int x, int y, Type gridObjectType, Direction direction = Direction.None )
	{
		if ( !_objects.ContainsKey( (x, y) ) )
			_objects.Add( (x, y), new List<(Type, Direction)>() );
		
		_objects[(x, y)].Add( (gridObjectType, direction) );
		return this;
	}

	public GridMap Build()
	{
		var grid = new GridMap
		{
			CellSize = _cellSize,
			Position = _worldPosition,
			Size = _size,
			StartPosition = _startPosition,
			Traverser = {ItemCapacity = _itemCap}
		};

		for ( var y = 0; y < grid.Size.Y; y++ )
		{
			for ( var x = 0; x < grid.Size.X; x++ )
			{
				grid.CellData.Add( new GridCell
				{
					GridMap = grid,
					GridPosition = new IntVector2( x, y ),
					CanMove =
					{
						[Direction.Up] = grid.IsValidPosition( x, y + 1 ),
						[Direction.Right] = grid.IsValidPosition( x + 1, y ),
						[Direction.Down] = grid.IsValidPosition( x, y - 1 ),
						[Direction.Left] = grid.IsValidPosition( x - 1, y )
					}
				} );
				
				if ( _items.TryGetValue( (x, y), out var itemType ) )
					grid.AddItemTo( x, y, TypeLibrary.Create<GridItem>( itemType ) );
			}
		}

		for ( var x = 0; x < grid.Size.X; x++ )
		{
			for ( var y = 0; y < grid.Size.Y; y++ )
			{
				if ( _objects.TryGetValue( (x, y), out var objects ) )
				{
					foreach ( var pair in objects )
						grid.PlaceObjectAt( x, y, pair.Item2, TypeLibrary.Create<GridObject>( pair.Item1 ) );
				}
			}
		}

		grid.Reset();
		return grid;
	}
}
