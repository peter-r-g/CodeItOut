using Sandbox.UI;
using Sandbox.UI.Construct;

namespace CodeItOut.UI;

public class InventoryIcon : Panel
{
	public Label Label;
	public Label Number;

	public InventoryIcon( int i, Panel parent )
	{
		Parent = parent;
		Label = Add.Label( "empty", "item-name" );
		Number = Add.Label( $"{i}", "slot-number" );
	}
}
