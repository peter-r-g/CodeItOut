using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeItOut.Grid.Traverser;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid;

public partial class GridEntity : AnimatedEntity
{
	[Net] public Grid Grid { get; set; }
	[Net] protected IntVector2 GridPosition { get; set; }
	[Net] protected Direction FacingDirection { get; set; }

	protected virtual Direction StartDirection => Direction.Up;

	protected GridCell CurrentGridCell => Grid.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo )
		? cellInfo
		: null;

	public int ActionCount => _svActions.Count;
	private readonly List<(TraverserAction, object[])> _svActions = new();
	
	public virtual void Reset()
	{
		Host.AssertServer();
		
		GridPosition = Grid.StartPosition;
		FacingDirection = StartDirection;
		_svActions.Clear();
	}

	public void AddAction( TraverserAction action, params object[] args )
	{
		Host.AssertServer();
		_svActions.Add( (action, args) );
	}

	public async Task<bool> RunAction( int actionIndex )
	{
		if ( actionIndex >= _svActions.Count )
			return false;

		var (action, args) = _svActions[actionIndex];
		return action switch
		{
			TraverserAction.MoveForward => await MoveForward(),
			TraverserAction.TurnLeft => await TurnLeft(),
			TraverserAction.TurnRight => await TurnRight(),
			TraverserAction.UseObject => await UseObject(),
			TraverserAction.UseItem => await UseItem( (double)args[0] ),
			TraverserAction.PickupItem => await PickupItem(),
			TraverserAction.DropItem => await DropItem( (double)args[0] ),
			_ => throw new ArgumentOutOfRangeException( nameof(action), action, null )
		};
	}

	protected virtual async Task<bool> MoveForward()
	{
		return false;
	}

	protected virtual async Task<bool> TurnLeft()
	{
		return false;
	}

	protected virtual async Task<bool> TurnRight()
	{
		return false;
	}

	protected virtual async Task<bool> UseObject()
	{
		return false;
	}

	protected virtual async Task<bool> UseItem( double itemIndex )
	{
		return false;
	}

	protected virtual async Task<bool> PickupItem()
	{
		return false;
	}

	protected virtual async Task<bool> DropItem( double itemIndex )
	{
		return false;
	}
}
