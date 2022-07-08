using CodeItOut.Grid;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut;

public partial class Pawn : Player
{
	[Net] public GridEntity Grid { get; private set; }

	public override void Spawn()
	{
		base.Spawn();

		Grid = new GridBuilder()
			.WithOwner( this )
			.WithWorldPosition( Vector3.Zero )
			.WithSize( 5, 5 )
			.WithStartPosition( 2, 0 )
			.WithCellSize( 100, 100 )
			.Build();

		Grid.AddItemTo( 2, 1, new KeyItem {KeyColor = Color.Green} );

		for ( var i = 0; i < 5; i++ )
		{
			Grid.PlaceObjectAt( 2, i, Direction.Left, new WallObject() );
			Grid.PlaceObjectAt( 2, i, Direction.Right, new WallObject() );
		}

		Grid.PlaceObjectAt( 2, 4, Direction.Up, new DoorObject {KeyColor = Color.Green} );

		Animator = null;
		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = Grid.Traverser,
			TargetOffset = new Vector3( 0, 0, 50 )
		};
		Controller = null;
		
		EnableDrawing = false;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		Grid?.Simulate( cl );

		if ( Grid is null || CameraMode is not LookAtCamera camera )
			return;

		camera.Origin = Grid.Traverser.Position + new Vector3( 0, -150, 150 );
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		Grid?.FrameSimulate( cl );
	}
}
