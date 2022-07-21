using CodeItOut.Grid;
using Sandbox;

namespace CodeItOut;

public partial class Pawn : Player
{
	[Net] public GridMap? Map { get; private set; }

	public override void Spawn()
	{
		base.Spawn();
		
		Map = GridMap.Load( FileSystem.Mounted, "maps/level0.s&s" );

		Animator = null;
		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = Map.Traverser,
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
