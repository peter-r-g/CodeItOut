using System.Collections.Generic;
using CodeItOut.Items;
using Sandbox.UI;

namespace CodeItOut.UI;

public class TraverserInventory : Panel
{
	public TraverserHud TraverserHud => Parent as TraverserHud;
	
	private readonly List<InventoryIcon> _slots = new();

	public override void Tick()
	{
		base.Tick();

		var traverser = TraverserHud.Traverser;
		var items = traverser.Items;
		for ( var i = _slots.Count; i < items.Count; i++ )
			_slots.Add( new InventoryIcon( i, this ) );
		
		for ( var i = 0; i < _slots.Count; i++ )
			UpdateIcon( i >= items.Count ? null : items[i], _slots[i] );
	}

	private void UpdateIcon( GridItem item, InventoryIcon inventoryIcon )
	{
		if ( item is null )
		{
			_slots.Remove( inventoryIcon );
			inventoryIcon.Delete();
			return;
		}

		inventoryIcon.Label.Text = item.ItemName;
	}
}
