using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using System.Threading;
using CodeItOut.Grid;
using Sandbox;
using SandScript;

namespace CodeItOut;

public class PuzzleGame : Game
{
	public new static PuzzleGame Current => Sandbox.Game.Current as PuzzleGame;
	public readonly PuzzleGameHud Hud;

	private CancellationTokenSource? _svRunCancelSource;

	public PuzzleGame()
	{
		if ( !Host.IsClient )
			return;
		
		Hud = new PuzzleGameHud();
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var pawn = new Pawn();
		client.Pawn = pawn;
		pawn.LoadCurrentLevel();
	}
	
	[ConCmd.Client( "play_game" )]
	public static void PlayGame()
	{
		var script = new Script();
		script.AddClassMethods<SandScriptInterop.Gameplay>();
		var returnValue = script.Execute( FileSystem.Data.ReadAllText( "input.s&s" ), out var diagnostics );
		
		foreach ( var info in diagnostics.Informationals )
			Log.Info( $"SandScript Information: {info}" );
		foreach ( var warning in diagnostics.Warnings )
			Log.Warning( $"SandScript Warning: {warning}" );
		foreach ( var error in diagnostics.Errors )
			Log.Warning( $"SandScript Error: {error}" );

		if ( returnValue is null )
			return;

		var json = JsonSerializer.Serialize( SandScriptInterop.Gameplay.ClActions );
		SandScriptInterop.Gameplay.ClActions.Clear();
		SubmitSolution( json );
	}

	[ConCmd.Server( "next_level" )]
	public static void NextLevel()
	{
		if ( ConsoleSystem.Caller.Pawn is not Pawn {GridMap.State: MapState.Won} pawn )
			return;

		pawn.NextLevel();
	}

	[ConCmd.Server( "restart_game" )]
	public static void RestartGame()
	{
		Current._svRunCancelSource?.Cancel();
		(ConsoleSystem.Caller.Pawn as Pawn)?.GridMap?.Reset();
	}
	
	[ConCmd.Server]
	// TODO: ServerRpc my beloved when :(
	public static void SubmitSolution( string data )
	{
		var map = (ConsoleSystem.Caller.Pawn as Pawn)?.GridMap;
		if ( map is null )
			return;
		
		var actions = JsonSerializer.Deserialize<List<TraverserAction>>( data );
		if ( actions is null )
			return;

		foreach ( var action in actions )
		{
			// TODO: Having an object array makes all arguments become JsonElements
			for ( var i = 0; i < action.ActionArguments.Length; i++ )
				action.ActionArguments[i] = ((JsonElement)action.ActionArguments[i]).GetInt32();
			
			map.Traverser.AddAction( action.ActionType, ImmutableArray.Create( action.ActionArguments ) );	
		}

		Current._svRunCancelSource = new CancellationTokenSource();
		_ = map.Run( Current._svRunCancelSource.Token );
	}
}
