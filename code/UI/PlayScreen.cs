using CodeItOut.Grid;
using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class PlayScreen : Panel
{
	public PlayScreen()
	{
		BindClass( "hidden", () => (Local.Pawn as Pawn)?.Map?.State != MapState.NotStarted );
	}
}
