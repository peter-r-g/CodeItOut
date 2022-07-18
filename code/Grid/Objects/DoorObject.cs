using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using Sandbox;

namespace CodeItOut.Grid;

public partial class DoorObject : GridObject
{
	protected override string ModelName => "models/maya_testcube_100.vmdl";

	[Net] public bool Open { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Scale = 0.1f;
	}

	public override bool Use( GridTraverser user, GridItem usedItem, out bool itemUsed )
	{
		itemUsed = false;
		if ( usedItem is null && Open )
		{
			Log.Info( "You win!" );
			return true;
		}
		
		if ( usedItem is not KeyItem )
			return false;

		Open = !Open;
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
		
		DebugOverlay.Text( $"Door(Open: {Open})", Position, Color.White, 0.1f );
	}
}
