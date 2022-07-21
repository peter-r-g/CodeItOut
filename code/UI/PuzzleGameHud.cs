using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class PuzzleGameHud : RootPanel
{
	public override void Tick()
	{
		base.Tick();

		var devCam = Local.Client.Components.Get<DevCamera>();
		SetClass( "active", devCam is null );
	}
	[ConCmd.Client( "play_game" )]
	public static void PlayGame()
	{
		Log.Info( "Sending:" );
		var text = FileSystem.Data.ReadAllText( "input.s&s" );
		Log.Info( text );
		PuzzleGame.SubmitSolution( text );
	}
}
