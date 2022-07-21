using Sandbox;

namespace CodeItOut.Grid;

public class FloorObject : GridObject
{
	protected override string ModelName => "models/maya_testcube_100.vmdl";
	public override Vector3 Offset => new(0, 0, -100);

	[Event.Tick.Client]
	private void Tick()
	{
		DebugOverlay.Box( this, Color.Black, 0.1f );
	}
}
