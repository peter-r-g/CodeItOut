using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeItOut.Items;
using CodeItOut.UI;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid.Traverser;

public partial class GridTraverser : GridEntity
{
	[Net] public IList<TraverserItem> Items { get; private set; }

	private const float TurnTime = 0.3f;
	private Direction _svPreviousDirection;
	private TimeSince _svTimeSinceTurnStart;

	private const float TravelTime = 1f;
	private IntVector2 _svPreviousGridPosition;
	private TimeSince _svTimeSinceMoveStart;
	
	public override void Spawn()
	{
		base.Spawn();
		
		SetModel( "models/citizen/citizen.vmdl" );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		TraverserHud.Instance.Traverser = this;
	}

	[Event.Tick.Server]
	protected void Tick()
	{
		UpdatePosition();
		UpdateRotation();
	}

	public override void Reset()
	{
		base.Reset();

		_svPreviousGridPosition = GridPosition;
		_svPreviousDirection = FacingDirection;
	}

	private void UpdatePosition()
	{
		Host.AssertServer();
		
		var previousPosition = _svPreviousGridPosition;
		if ( !Grid.TryGetCellAt( previousPosition.X, previousPosition.Y, out var previousCellInfo ) )
		{
			Log.Error( $"Failed to get cell at {previousPosition.X}, {previousPosition.Y}" );
			return;
		}
		
		var currentPosition = GridPosition;
		if ( !Grid.TryGetCellAt( currentPosition.X, currentPosition.Y, out var cellInfo ) )
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
		
		Rotation = Rotation.Lerp( _svPreviousDirection.ToRotation(), FacingDirection.ToRotation(),
			_svTimeSinceTurnStart / TurnTime );
	}
	
	protected override async Task<bool> TurnLeft()
	{
		Host.AssertServer();

		var newValue = (byte)FacingDirection - 1;
		if ( newValue < (byte)Direction.Up )
			newValue = (byte)Direction.Left;

		_svPreviousDirection = FacingDirection;
		FacingDirection = (Direction)newValue;
		_svTimeSinceTurnStart = 0;
		await GameTask.DelaySeconds( TurnTime + 0.5f );
		return true;
	}

	protected override async Task<bool> TurnRight()
	{
		Host.AssertServer();
		
		var newValue = (byte)FacingDirection + 1;
		if ( newValue > (byte)Direction.Left )
			newValue = (byte)Direction.Up;

		_svPreviousDirection = FacingDirection;
		FacingDirection = (Direction)newValue;
		_svTimeSinceTurnStart = 0;
		await GameTask.DelaySeconds( TurnTime + 0.5f );
		return true;
	}

	protected override async Task<bool> MoveForward()
	{
		Host.AssertServer();
		
		var newGridPosition = GridPosition + FacingDirection.ToPosition();
		newGridPosition = newGridPosition.Clamp( IntVector2.Zero, Grid.Size - IntVector2.One );

		if ( GridPosition == newGridPosition )
			return false;
		
		if ( !CurrentGridCell.CanMove[FacingDirection] )
			return false;

		_svPreviousGridPosition = GridPosition;
		GridPosition = newGridPosition;
		_svTimeSinceMoveStart = 0;
		await GameTask.DelaySeconds( TravelTime + 0.5f );
		return true;
	}

	protected override async Task<bool> UseObject()
	{
		Host.AssertServer();

		var cellInfo = CurrentGridCell;
		if ( cellInfo is null )
			return false;
		
		foreach ( var obj in cellInfo.GetObjectsInDirection( FacingDirection ) )
		{
			if ( obj.Use( this, null ) )
				return true;
		}

		return false;
	}

	protected override async Task<bool> UseItem( double itemIndex )
	{
		Host.AssertServer();

		var cellInfo = CurrentGridCell;
		if ( cellInfo is null )
			return false;
		
		if ( Items.Count < itemIndex )
			return false;

		foreach ( var obj in cellInfo.GetObjectsInDirection( FacingDirection ) )
		{
			if ( obj.Use( this, Items[(int)itemIndex] ) )
				return true;
		}

		return false;
	}

	protected override async Task<bool> PickupItem()
	{
		Host.AssertServer();

		var cellInfo = CurrentGridCell;
		if ( cellInfo is null )
			return false;

		if ( cellInfo.GroundItem is null )
			return false;

		cellInfo.GroundItem.OnPickup( this );
		return true;
	}

	protected override async Task<bool> DropItem( double itemIndex )
	{
		Host.AssertServer();

		var cellInfo = CurrentGridCell;
		if ( cellInfo is null )
			return false;

		if ( cellInfo.GroundItem is not null )
			return false;

		if ( Items.Count < itemIndex )
			return false;
		
		Items[(int)itemIndex].OnDrop( cellInfo );
		return true;
	}
}
