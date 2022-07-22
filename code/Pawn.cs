using CodeItOut.Grid;
using Sandbox;

namespace CodeItOut;

public partial class Pawn : Player
{
	[ConVar.Client( "grid_debug" )]
	public static bool GridDebugEnabled { get; set; }
	
	[Net] public int Level { get; private set; }
	[Net] public GridMap? Map { get; private set; }

	public override void Spawn()
	{
		base.Spawn();
		
		LoadCurrentLevel();

		Animator = null;
		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = Map?.Traverser,
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

		if ( Map is not null )
		{
			if ( Map.State == MapState.Running && CameraMode is LookAtCamera camera )
				camera.Origin = Map.Traverser.Position + new Vector3( 0, -150, 150 );
		}

		if ( GridDebugEnabled )
			Map?.FrameSimulate( cl );
	}

	public void NextLevel()
	{
		Level++;
		Map?.Cleanup();
		LoadCurrentLevel();
	}

	private void LoadCurrentLevel()
	{
		Host.AssertServer();
		Map = GridMap.Load( FileSystem.Mounted, $"maps/level{Level}.s&s" );
		Map.Reset();
	}

	[GridEvent.MapReady.Server]
	private void MapReady( GridMap map )
	{
		if ( map != Map )
			return;

		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = map.Traverser,
			TargetOffset = new Vector3( 0, 0, 50 )
		};
	}

	[GridEvent.MapStart.Server]
	private void MapStart( GridMap map )
	{
		if ( map != Map )
			return;
		
		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = map.Traverser,
			TargetOffset = new Vector3( 0, 0, 50 )
		};
	}
}
