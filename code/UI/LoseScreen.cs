using CodeItOut.Grid;
using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class LoseScreen : Panel
{
	public LoseScreen()
	{
		BindClass( "hidden", () => (Local.Pawn as Pawn)?.Map?.State != MapState.Lost );
	}
}
