using CodeItOut.Grid;
using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class WinScreen : Panel
{
	public WinScreen()
	{
		BindClass( "hidden", () => (Local.Pawn as Pawn)?.GridMap?.State != MapState.Won );
	}
}
