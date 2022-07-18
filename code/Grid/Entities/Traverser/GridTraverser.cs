using System;
using CodeItOut.Items;
using CodeItOut.UI;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid.Traverser;

public class GridTraverser : GridEntity
{
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

	public override void Reset()
	{
		base.Reset();
		
		Items.Clear();
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

		return ActionResult.Success( newGridPosition, Direction, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected override ActionResult TurnLeft()
	{
		var newDirection = (byte)Direction - 1;
		if ( newDirection < (byte)Direction.Up )
			newDirection = (byte)Direction.Left;

		return ActionResult.Success( GridPosition, (Direction)newDirection, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected override ActionResult TurnRight()
	{
		var newDirection = (byte)Direction + 1;
		if ( newDirection > (byte)Direction.Left )
			newDirection = (byte)Direction.Up;

		return ActionResult.Success( GridPosition, (Direction)newDirection, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}

	protected override ActionResult UseObject()
	{
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return ActionResult.Fail();

		foreach ( var obj in cellInfo.GetObjectsInDirection( Direction ) )
		{
			if ( obj.Use( this, null, out _ ) )
				return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
					ArraySegment<GridItem>.Empty );
		}

		return ActionResult.Fail();
	}

	protected override ActionResult UseItem( double itemIndex )
	{
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return ActionResult.Fail();
		
		if ( Items.Count < itemIndex )
			return ActionResult.Fail();

		foreach ( var obj in cellInfo.GetObjectsInDirection( Direction ) )
		{
			var item = Items[(int)itemIndex];
			if ( obj.Use( this, item, out var itemUsed ) )
				return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
					itemUsed ? new[] {item} : ArraySegment<GridItem>.Empty );
		}

		return ActionResult.Fail();
	}

	protected override ActionResult PickupItem()
	{
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return ActionResult.Fail();

		if ( cellInfo.GroundItem is null )
			return ActionResult.Fail();

		cellInfo.GroundItem.OnPickup( this );
		return ActionResult.Success( GridPosition, Direction, new[] {cellInfo.GroundItem},
			ArraySegment<GridItem>.Empty );
	}

	protected override ActionResult DropItem( double itemIndex )
	{
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return ActionResult.Fail();

		if ( cellInfo.GroundItem is not null )
			return ActionResult.Fail();

		if ( Items.Count < itemIndex )
			return ActionResult.Fail();
		
		Items[(int)itemIndex].OnDrop( cellInfo );
		return ActionResult.Success( GridPosition, Direction, ArraySegment<GridItem>.Empty,
			new[] {cellInfo.GroundItem} );
	}
}
