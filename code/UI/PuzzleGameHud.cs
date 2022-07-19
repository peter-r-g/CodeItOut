using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class PuzzleGameHud : RootPanel
{
	private string _text = string.Empty;

	public PuzzleGameHud()
	{
		PuzzleGame.Current.InputFileWatch.OnChangedFile += InputFileWatchOnOnChangedFile;
	}

	public override void Tick()
	{
		base.Tick();

		var devCam = Local.Client.Components.Get<DevCamera>();
		SetClass( "active", devCam is null );
	}

	
	public void Play()
	{
		if ( string.IsNullOrWhiteSpace( _text ) )
			return;

		PuzzleGame.SubmitSolution( _text );
	}
	
	private void InputFileWatchOnOnChangedFile( string text )
	{
		_text = text;
	}
}
