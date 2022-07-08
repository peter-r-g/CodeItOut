using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid;

public class GridBuilder
{
	private IntVector2 _cellSize;
	private Entity _owner;
	private IntVector2 _size;
	private IntVector2 _startPosition;
	private Vector3 _worldPosition;
	
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

	public GridBuilder WithOwner( Entity owner )
	{
		_owner = owner;
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

	public GridBuilder WithWorldPosition( Vector3 position )
	{
		_worldPosition = position;
		return this;
	}

	public GridEntity Build()
	{
		var grid = new GridEntity
		{
			CellSize = _cellSize,
			Owner = _owner,
			Position = _worldPosition,
			Size = _size,
			StartPosition = _startPosition
		};
		
		for ( var y = 0; y < grid.Size.Y; y++ )
		{
			for ( var x = 0; x < grid.Size.X; x++ )
			{
				grid.CellData.Add( new GridCell
				{
					Grid = grid,
					GridPosition = new IntVector2( x, y ),
					CanMove =
					{
						[Direction.Up] = grid.IsValidPosition( x, y + 1 ),
						[Direction.Right] = grid.IsValidPosition( x + 1, y ),
						[Direction.Down] = grid.IsValidPosition( x, y - 1 ),
						[Direction.Left] = grid.IsValidPosition( x - 1, y )
					}
				} );
			}
		}

		grid.Reset();
		return grid;
	}
}
