using CodeItOut.Grid;
using Sandbox;
using Sandbox.UI;

namespace CodeItOut;

[UseTemplate]
public class RunningScreen : Panel
{
	public RunningScreen()
	{
		BindClass( "hidden", () => (Local.Pawn as Pawn)?.Map?.State != MapState.Running );
	}
}
