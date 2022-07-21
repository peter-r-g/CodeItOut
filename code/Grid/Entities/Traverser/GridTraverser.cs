using System;
using CodeItOut.Items;
using CodeItOut.UI;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid.Traverser;

public class GridTraverser : GridEntity
{
	private const float CenterDeadzone = 0.01f;
	private const int MoveSpeed = 100;
	
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

	protected override void UpdatePosition()
	{
		base.UpdatePosition();
		
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
		{
			Log.Error( $"Failed to get cell at {GridPosition.X}, {GridPosition.Y}" );
			return;
		}
		
		var animation = new CitizenAnimationHelper( this );
		if ( Position.Distance( cellInfo.WorldPositionCenter ) < CenterDeadzone )
		{
			animation.WithVelocity( Vector3.Zero );
			return;
		}

		animation.WithVelocity( Rotation.Forward.Normal * MoveSpeed );
	}
	
	protected override ActionResult MoveForward()
	{
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return ActionResult.Fail();
			
		if ( !cellInfo.CanMove[Direction] )
			return ActionResult.Fail();
		
		var newGridPosition = GridPosition + Direction.ToPosition();
		if ( !GridMap.TryGetCellAt( newGridPosition.X, newGridPosition.Y, out var targetCellInfo ) )
			return ActionResult.Fail();
		
		if ( !targetCellInfo.CanMove[Direction.Opposite()] )
			return ActionResult.Fail();

		return ActionResult.Success( newGridPosition, Direction );
	}

	protected override ActionResult TurnLeft()
	{
		var newDirection = (byte)Direction - 1;
		if ( newDirection < (byte)Direction.Up )
			newDirection = (byte)Direction.Left;

		return ActionResult.Success( GridPosition, (Direction)newDirection );
	}

	protected override ActionResult TurnRight()
	{
		var newDirection = (byte)Direction + 1;
		if ( newDirection > (byte)Direction.Left )
			newDirection = (byte)Direction.Up;

		return ActionResult.Success( GridPosition, (Direction)newDirection );
	}

	protected override ActionResult UseObject()
	{
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return ActionResult.Fail();

		foreach ( var obj in cellInfo.GetObjectsInDirection( Direction ) )
		{
			if ( obj.Use( this, null, out _ ) )
				return ActionResult.Success( GridPosition, Direction );
		}

		return ActionResult.Fail();
	}

	protected override ActionResult UseItem( int indexToUse )
	{
		if ( !TryUseItem( indexToUse, out var usedItem, out var itemUsed ) )
			return ActionResult.Fail();

		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			itemUsed ? new[] {usedItem} : ArraySegment<GridItem>.Empty );
	}

	protected override ActionResult PickupItem( int indexToPlaceIn )
	{
		return !TryPickupItem( indexToPlaceIn, out var pickedUpItem )
			? ActionResult.Fail()
			: ActionResult.Success( GridPosition, Direction, new[] {pickedUpItem} );
	}

	protected override ActionResult DropItem( int indexToDrop )
	{
		return !TryDropItem( indexToDrop, out var droppedItem )
			? ActionResult.Fail()
			: ActionResult.Success( GridPosition, Direction, null, new[] {droppedItem} );
	}
}
