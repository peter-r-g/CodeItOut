using CodeItOut.Grid;
using CodeItOut.Items;
using Sandbox;

namespace CodeItOut;

public partial class Pawn : Player
{
	[Net] public Grid.Grid Grid { get; private set; }

	public override void Spawn()
	{
		base.Spawn();

		Grid = new GridBuilder()
			.WithOwner( this )
			.WithWorldPosition( Vector3.Zero )
			.WithSize( 1, 5 )
			.WithStartPosition( 0, 0 )
			.WithCellSize( 100, 100 )
			.Build();

		var floorColor = Color.White;
		for ( var x = 0; x < Grid.Size.X; x++ )
		{
			for ( var y = 0; y < Grid.Size.Y; y++ )
			{
				var floor = new FloorObject {RenderColor = floorColor};
				Grid.PlaceObjectAt( x, y, Direction.None, floor );
				floorColor = floorColor == Color.White ? Color.Magenta : Color.White;
			}
		}

		Grid.AddItemTo( 0, 2, new KeyItem {KeyColor = Color.Green} );

		Grid.PlaceObjectAt( 0, 0, Direction.Down, new WallObject() );
		for ( var i = 0; i < 5; i++ )
		{
			Grid.PlaceObjectAt( 0, i, Direction.Left, new WallObject() );
			Grid.PlaceObjectAt( 0, i, Direction.Right, new WallObject() );
		}

		Grid.PlaceObjectAt( 0, 4, Direction.Up, new DoorObject {KeyColor = Color.Green} );

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
		if ( Map is null || CameraMode is not LookAtCamera camera )
			return;

		camera.Origin = Map.Traverser.Position + new Vector3( 0, -150, 150 );
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
		
		if ( GridDebugEnabled )
			Map?.FrameSimulate( cl );
	}

	[ConVar.Client( "grid_debug" )]
	public static bool GridDebugEnabled { get; set; }
}
