using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid.Traverser;

public partial class GridTraverser : AnimatedEntity
{
	[Net] public GridEntity Grid { get; set; }
	[Net] public IntVector2 GridPosition { get; set; }
	[Net] public Direction FacingDirection { get; set; } = Direction.Up;
	[Net] public IList<TraverserItem> Items { get; private set; }

	public GridCell CurrentGridCell
	{
		get => Grid.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) ? cellInfo : null;
	}

	private readonly List<(TraverserAction, object[])> _svActions = new();

	public override void Spawn()
	{
		base.Spawn();
		
		SetModel( "models/citizen/citizen.vmdl" );
	}

	[Event.Tick.Server]
	protected void Tick()
	{
		UpdatePosition();
		UpdateRotation();

		DebugOverlay.Line( Position, Position + new Vector3( FacingDirection.ToPosition() * 100, 0 ), Color.Magenta, 0.1f );
	}

	public void Reset()
	{
		Host.AssertServer();
		
		GridPosition = Grid.StartPosition;
		FacingDirection = Direction.Up;
		_svActions.Clear();
	}

	public void AddAction( TraverserAction action, params object[] args )
	{
		Host.AssertServer();
		_svActions.Add( (action, args) );
	}

	public async Task RunActions()
	{
		Host.AssertServer();
		
		foreach ( var action in _svActions )
		{
			switch ( action.Item1 )
			{
				case TraverserAction.MoveForward:
					if ( !MoveForward() )
					{
						await End();
						return;
					}
					
					break;
				case TraverserAction.TurnLeft:
					TurnLeft();
					break;
				case TraverserAction.TurnRight:
					TurnRight();
					break;
				case TraverserAction.UseItem:
					if ( !UseItem( (double)action.Item2[0] ) )
					{
						await End();
						return;
					}
					
					break;
				case TraverserAction.PickupItem:
					if ( !PickupItem() )
					{
						await End();
						return;
					}
					
					break;
				case TraverserAction.DropItem:
					if ( !DropItem( (double)action.Item2[0] ) )
					{
						await End();
						return;
					}
					
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			await GameTask.DelaySeconds( 0.5f );
		}

		await End();
	}

	private async Task End()
	{
		await GameTask.DelaySeconds( 2 );
		Grid.Reset();
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
			_ => throw new ArgumentOutOfRangeException()
		};
	}
	
	private void TurnLeft()
	{
		Host.AssertServer();

		var newValue = (byte)FacingDirection - 1;
		if ( newValue < (byte)Direction.Up )
			newValue = (byte)Direction.Left;

		FacingDirection = (Direction)newValue;
	}

	private void TurnRight()
	{
		Host.AssertServer();
		
		var newValue = (byte)FacingDirection + 1;
		if ( newValue > (byte)Direction.Left )
			newValue = (byte)Direction.Up;
		
		FacingDirection = (Direction)newValue;
	}

	private bool MoveForward()
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

	private bool UseItem( double itemIndex )
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

	private bool PickupItem()
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

	private bool DropItem( double itemIndex )
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
