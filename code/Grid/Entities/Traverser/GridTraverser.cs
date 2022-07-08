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

	private void UpdatePosition()
	{
		Host.AssertServer();
		
		var gridPosition = GridPosition;
		if ( !Grid.TryGetCellAt( gridPosition.X, gridPosition.Y, out var cellInfo ) )
		{
			Log.Error( $"Failed to get cell at {gridPosition.X}, {gridPosition.Y}" );
			return;
		}
		
		Position = cellInfo.WorldPositionCenter;
	}

	private void UpdateRotation()
	{
		Host.AssertServer();
		
		Rotation = FacingDirection switch
		{
			Direction.Right => Rotation.From( 0, 0, 0 ),
			Direction.Up => Rotation.From( 0, 90, 0 ),
			Direction.Left => Rotation.From( 0, 180, 0 ),
			Direction.Down => Rotation.From( 0, 270, 0 ),
			Direction.None => Rotation.From( 0, 0, 0 ),
			_ => throw new ArgumentOutOfRangeException()
		};
	}
	
	protected override async Task<bool> TurnLeft()
	{
		Host.AssertServer();

		var newValue = (byte)FacingDirection - 1;
		if ( newValue < (byte)Direction.Up )
			newValue = (byte)Direction.Left;

		FacingDirection = (Direction)newValue;
		return true;
	}

	protected override async Task<bool> TurnRight()
	{
		Host.AssertServer();
		
		var newValue = (byte)FacingDirection + 1;
		if ( newValue > (byte)Direction.Left )
			newValue = (byte)Direction.Up;
		
		FacingDirection = (Direction)newValue;
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
			
		GridPosition = newGridPosition;
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
