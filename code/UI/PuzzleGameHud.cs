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
}
