using Sandbox;

namespace CodeItOut.Items;

public partial class KeyItem : TraverserItem
{
	public override string ItemName => "Key";
	protected override string ModelPath => "models/citizen_props/crowbar01.vmdl";
	
	[Net] public Color KeyColor { get; set; }

	public override void DebugDraw()
	{
		base.DebugDraw();
		
		DebugOverlay.Text( "Key", Position, KeyColor, 0.1f );
	}
}
