using CodeItOut.Grid;
using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class RunningScreen : Panel
{
	public RunningScreen()
	{
		BindClass( "hidden", () => (Local.Pawn as Pawn)?.GridMap?.State != MapState.Running );
	}
}
