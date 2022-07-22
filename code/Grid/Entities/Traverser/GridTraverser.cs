using CodeItOut.UI;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid.Traverser;

public partial class GridTraverser : GridEntity
{
	private const float CenterDeadzone = 0.01f;
	private const int MoveSpeed = 100;
	
	[Net] private ModelEntity? Ragdoll { get; set; }

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

	[GridEvent.MapLost.Server]
	private void PlayLoseAnimation( GridMap map )
	{
		if ( map != GridMap )
			return;
		
		BecomeRagdoll();
	}

	private void BecomeRagdoll()
	{
		var ent = new ModelEntity
		{
			EnableAllCollisions = true,
			Position = Position,
			Rotation = Rotation,
			Scale = Scale,
			UsePhysicsCollision = true,
			PhysicsEnabled = true,
			SurroundingBoundsMode = SurroundingBoundsType.Physics,
			RenderColor = RenderColor
		};
		ent.Tags.Add( "ragdoll", "solid", "debris" );
		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.CopyBodyGroups( this );
		ent.CopyMaterialGroup( this );
		ent.CopyMaterialOverrides( this );
		ent.TakeDecalsFrom( this );

		foreach ( var child in Children )
		{
			if ( !child.Tags.Has( "clothes" ) ) continue;
			if ( child is not ModelEntity e ) continue;

			var model = e.GetModelName();

			var clothing = new ModelEntity {RenderColor = e.RenderColor};
			clothing.SetModel( model );
			clothing.SetParent( ent, true );
			clothing.CopyBodyGroups( e );
			clothing.CopyMaterialGroup( e );
		}

		Ragdoll = ent;
		EnableDrawing = false;
	}

	public override void Reset()
	{
		base.Reset();
		
		Ragdoll?.Delete();
		EnableDrawing = true;
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

		return !targetCellInfo.CanMove[Direction.Opposite()]
			? ActionResult.Fail()
			: ActionResult.Success( newGridPosition, Direction );
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

	protected override ActionResult UseObject( int? itemIndexToUse )
	{
		return !TryUseObject( out _, out _, itemIndexToUse )
			? ActionResult.Fail()
			: ActionResult.Success( GridPosition, Direction );
	}

	protected override ActionResult UseItem( int indexToUse )
	{
		return !TryUseItem( indexToUse, out _, out _ )
			? ActionResult.Fail()
			: ActionResult.Success( GridPosition, Direction );
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
