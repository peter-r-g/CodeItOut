using CodeItOut.Grid;
using Sandbox;

namespace CodeItOut;

public partial class Pawn : Player
{
	[ConVar.Client( "grid_debug" )]
	public static bool GridDebugEnabled { get; set; }
	
	[Net] public int Level { get; private set; }
	public GridMap? GridMap
	{
		get => Map;
		set
		{
			var oldGridMap = Map;
			Map = value;
			OnGridMapChanged( oldGridMap, value );
		}
	}
	[Net] private GridMap? Map { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Animator = null;
		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = GridMap?.Traverser,
			TargetOffset = new Vector3( 0, 0, 50 )
		};
		Controller = null;
		EnableDrawing = false;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		if ( GridMap is null )
			return;
		
		GridMap.Simulate( cl );
		
		if ( CameraMode is LookAtCamera camera )
			camera.Origin = GridMap.Traverser.Position + new Vector3( 0, -150, 150 );
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		if ( GridMap is not null )
		{
			if ( GridMap.State == MapState.Running && CameraMode is LookAtCamera camera )
				camera.Origin = GridMap.Traverser.Position + new Vector3( 0, -150, 150 );
		}

		if ( GridDebugEnabled )
			GridMap?.FrameSimulate( cl );
	}
	
	private void OnGridMapChanged( GridMap? oldGridMap, GridMap? newGridMap )
	{
		if ( oldGridMap is not null )
		{
			oldGridMap.MapReady -= MapReady;
			oldGridMap.MapStart -= MapStart;
		}

		if ( newGridMap is not null )
		{
			newGridMap.MapReady += MapReady;
			newGridMap.MapStart += MapStart;
		}
	}

	public void NextLevel()
	{
		Host.AssertServer();
		
		Level++;
		GridMap?.Cleanup();
		LoadCurrentLevel();
	}

	public void LoadCurrentLevel()
	{
		Host.AssertServer();
		
		GridMap = GridMap.Load( FileSystem.Mounted, $"maps/level{Level}.s&s" );
		GridMap.Reset();
		
		var clothing = new ClothingContainer();
		clothing.LoadFromClient( Client );
		clothing.DressEntity( GridMap.Traverser );
	}
	
	private void MapReady()
	{
		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = GridMap!.Traverser,
			TargetOffset = new Vector3( 0, 0, 50 )
		};
	}
	
	private void MapStart()
	{
		CameraMode = new LookAtCamera
		{
			LerpSpeed = 0,
			TargetEntity = GridMap!.Traverser,
			TargetOffset = new Vector3( 0, 0, 50 )
		};
	}
}
