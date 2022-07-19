using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid;

public partial class GridEntity : AnimatedEntity
{
	[Net] public GridMap GridMap { get; set; }
	[Net] public IntVector2 GridPosition { get; set; }
	[Net] public Direction Direction { get; private set; }
	[Net] private IList<GridItem> Items { get; set; }
	[Net] public int ItemCapacity { get; set; }

	protected virtual Direction StartDirection => Direction.Up;

	public int ActionCount => _svActions.Count;
	private readonly List<(TraverserAction, object[])> _svActions = new();
	
	private const float TurnTime = 0.3f;
	private Direction _svPreviousDirection = Direction.None;
	private TimeSince _svTimeSinceTurnStart;

	private const float TravelTime = 1f;
	private IntVector2 _svPreviousGridPosition = new(-1, -1);
	private TimeSince _svTimeSinceMoveStart;
	
	[Event.Tick.Server]
	protected void Tick()
	{
		if ( _svPreviousGridPosition != new IntVector2( -1, -1 ) )
			UpdatePosition();
		if ( _svPreviousDirection != Direction.None )
			UpdateRotation();
	}
	
	private void UpdatePosition()
	{
		Host.AssertServer();
		
		var previousPosition = _svPreviousGridPosition;
		if ( !GridMap.TryGetCellAt( previousPosition.X, previousPosition.Y, out var previousCellInfo ) )
		{
			Log.Error( $"Failed to get cell at {previousPosition.X}, {previousPosition.Y}" );
			return;
		}
		
		var currentPosition = GridPosition;
		if ( !GridMap.TryGetCellAt( currentPosition.X, currentPosition.Y, out var cellInfo ) )
		{
			Log.Error( $"Failed to get cell at {currentPosition.X}, {currentPosition.Y}" );
			return;
		}

		Position = Vector3.Lerp( previousCellInfo.WorldPositionCenter, cellInfo.WorldPositionCenter,
			_svTimeSinceMoveStart / TravelTime );
	}

	private void UpdateRotation()
	{
		Host.AssertServer();
		
		Rotation = Rotation.Lerp( _svPreviousDirection.ToRotation(), Direction.ToRotation(),
			_svTimeSinceTurnStart / TurnTime );
	}
	
	public virtual void Reset()
	{
		Host.AssertServer();
		
		GridPosition = GridMap.StartPosition;
		Direction = StartDirection;
		_svActions.Clear();
		_svPreviousGridPosition = GridPosition;
		_svPreviousDirection = Direction;
	}

	public void AddAction( TraverserAction action, params object[] args )
	{
		Host.AssertServer();
		_svActions.Add( (action, args) );
	}

	public async Task<ActionState> RunAction( int actionIndex )
	{
		if ( actionIndex >= _svActions.Count )
			return ActionState.Succeeded;

		var (action, args) = _svActions[actionIndex];
		var actionResult = action switch
		{
			TraverserAction.MoveForward => MoveForward(),
			TraverserAction.TurnLeft => TurnLeft(),
			TraverserAction.TurnRight => TurnRight(),
			TraverserAction.UseObject => UseObject(),
			TraverserAction.UseItem when args.Length == 1 => UseItem( (int)args[0] ),
			TraverserAction.PickupItem when args.Length == 1 => PickupItem( (int)args[0] ),
			TraverserAction.DropItem when args.Length == 1 => DropItem( (int)args[0] ),
			TraverserAction.Wait => Wait(),
			_ => throw new ArgumentOutOfRangeException( nameof(action), action, null )
		};
		if ( actionResult.ActionState == ActionState.Failed )
			return ActionState.Failed;

		if ( actionResult.GridPosition != GridPosition )
		{
			_svPreviousGridPosition = GridPosition;
			GridPosition = actionResult.GridPosition;
			_svTimeSinceMoveStart = 0;
			await GameTask.DelaySeconds( TravelTime );
		}
		else if ( actionResult.Direction != Direction )
		{
			_svPreviousDirection = Direction;
			Direction = actionResult.Direction;
			_svTimeSinceTurnStart = 0;
			await GameTask.DelaySeconds( TurnTime );
		}

		await GameTask.DelaySeconds( 0.5f );
		return actionResult.ActionState;
	}

	protected virtual ActionResult MoveForward()
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected virtual ActionResult TurnLeft()
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected virtual ActionResult TurnRight()
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected virtual ActionResult UseObject()
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected virtual ActionResult UseItem( int indexToUse )
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected virtual ActionResult PickupItem( int indexToPlaceIn )
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected virtual ActionResult DropItem( int indexToDrop )
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}
	
	protected virtual ActionResult Wait()
	{
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}
}
