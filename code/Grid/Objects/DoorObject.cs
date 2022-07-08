using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using Sandbox;

namespace CodeItOut.Grid;

public partial class DoorObject : GridObject
{
	public override string ModelName => "models/maya_testcube_100.vmdl";

	[Net] public bool Open { get; set; }
	[Net] public Color KeyColor { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Scale = 0.1f;
	}

	[Event.Tick.Server]
	protected void Tick()
	{
		RenderColor = KeyColor;
	}

	public override bool Use( GridTraverser user, TraverserItem usedItem )
	{
		if ( usedItem is not KeyItem key || key.KeyColor != KeyColor )
			return false;

		Open = true;
		return true;

	}

	public override bool IsObstructing()
	{
		return !Open;
	}

	public override void Reset()
	{
		base.Reset();

		Open = false;
	}

	public override void DebugDraw()
	{
		base.DebugDraw();
		
		DebugOverlay.Text( $"Door(Open: {Open})", Position, KeyColor, 0.1f );
	}
}
