using Sandbox;
using SandScript;

namespace CodeItOut;

public partial class PuzzleGame : Game
{
	public new static PuzzleGame Current => Sandbox.Game.Current as PuzzleGame;

	[Net] public PuzzleGameHud Hud { get; private set; }

	public PuzzleGame()
	{
		SandScript.SandScript.RegisterClassMethods();
		
		if ( Host.IsClient )
			Hud = new PuzzleGameHud();
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var pawn = new Pawn();
		client.Pawn = pawn;
		
		var clothing = new ClothingContainer();
		clothing.LoadFromClient( client );
		clothing.DressEntity( pawn.Map.Traverser );
	}

	[ConCmd.Server]
	public static void SubmitSolution( string text )
	{
		var script = new Script();
		script.AddClassMethods<SandScriptInterop.Gameplay>();
		Script.Execute( text, out var returnValue );
		if ( returnValue is not null )
			(ConsoleSystem.Caller.Pawn as Pawn)?.Map.Run();
	}
}
