using CodeItOut.Grid.Traverser;
using Sandbox.UI;

namespace CodeItOut.UI;

[UseTemplate]
public class TraverserHud : Panel
{
	public static TraverserHud Instance;
	
	public GridTraverser Traverser { get; set; }

	public TraverserHud()
	{
		Instance = this;
	}

	public override void Tick()
	{
		if ( Traverser is null )
			return;
		
		base.Tick();
	}
}
