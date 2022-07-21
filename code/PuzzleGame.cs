using Sandbox;
using SandScript;

namespace CodeItOut;

public class PuzzleGame : Game
{
	public new static PuzzleGame Current => Sandbox.Game.Current as PuzzleGame;
	public readonly PuzzleGameHud Hud;

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
		
		var clothing = new ClothingContainer();
		clothing.LoadFromClient( client );
		clothing.DressEntity( pawn.Map?.Traverser );
	}
	
	[ConCmd.Client( "play_game" )]
	public static void PlayGame()
	{
		var text = FileSystem.Data.ReadAllText( "input.s&s" );
		SubmitSolution( text );
	}

	[ConCmd.Server( "restart_game" )]
	public static void RestartGame()
	{
		(ConsoleSystem.Caller.Pawn as Pawn)?.Map?.Reset();
	}

	[ConCmd.Server]
	public static void SubmitSolution( string text )
	{
		var script = new Script();
		script.AddClassMethods<SandScriptInterop.Gameplay>();
		var returnValue = script.Execute( text, out var diagnostics );
		
		foreach ( var info in diagnostics.Informationals )
			Log.Info( $"SandScript Information: {info}" );
		foreach ( var warning in diagnostics.Warnings )
			Log.Warning( $"SandScript Warning: {warning}" );
		foreach ( var error in diagnostics.Errors )
			Log.Warning( $"SandScript Error: {error}" );
		
		if ( returnValue is not null )
			(ConsoleSystem.Caller.Pawn as Pawn)?.Map?.Run();
	}
}
