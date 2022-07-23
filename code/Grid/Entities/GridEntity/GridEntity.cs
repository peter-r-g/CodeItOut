using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid;

public abstract partial class GridEntity : AnimatedEntity
{
	[Net] public GridMap GridMap { get; set; }
	[Net] public IntVector2 GridPosition { get; set; }
	[Net] public Direction Direction { get; private set; }

	protected virtual Direction StartDirection => Direction.Up;

	public int ActionCount => _svActions.Count;
	private readonly List<(TraverserActionType, ImmutableArray<object>)> _svActions = new();
	
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
	
	protected virtual void UpdatePosition()
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

	protected virtual void UpdateRotation()
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
		Items.Clear();
		_svActions.Clear();
		_svPreviousGridPosition = GridPosition;
		_svPreviousDirection = Direction;
	}

	public void AddAction( TraverserActionType actionType, ImmutableArray<object> args )
	{
		Host.AssertServer();
		_svActions.Add( (actionType, args) );
	}

	public async Task<ActionState> RunAction( int actionIndex )
	{
		if ( actionIndex >= _svActions.Count )
			return ActionState.Succeeded;

		var (action, args) = _svActions[actionIndex];
		var actionResult = action switch
		{
			TraverserActionType.MoveForward => MoveForward(),
			TraverserActionType.TurnLeft => TurnLeft(),
			TraverserActionType.TurnRight => TurnRight(),
			TraverserActionType.UseObject => UseObject( args.Length == 1 ? (int)args[0] : null ),
			TraverserActionType.UseItem when args.Length == 1 => UseItem( (int)args[0] ),
			TraverserActionType.PickupItem when args.Length == 1 => PickupItem( (int)args[0] ),
			TraverserActionType.DropItem when args.Length == 1 => DropItem( (int)args[0] ),
			TraverserActionType.Wait => Wait(),
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

	protected abstract ActionResult MoveForward();
	protected abstract ActionResult TurnLeft();
	protected abstract ActionResult TurnRight();
	protected abstract ActionResult UseObject( int? itemIndexToUse );
	protected abstract ActionResult UseItem( int indexToUse );
	protected abstract ActionResult PickupItem( int indexToPlaceIn );
	protected abstract ActionResult DropItem( int indexToDrop );
	protected abstract ActionResult Wait();
}
