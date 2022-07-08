using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class PuzzleGameHud : RootPanel
{
	[Event.BuildInput]
	protected void BuildInput( InputBuilder input )
	{
		var devCam = Local.Client.Components.Get<DevCamera>();
		SetClass( "active", devCam is null );
	}
}
