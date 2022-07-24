using CodeItOut.Items;
using Sandbox;

namespace CodeItOut.Grid;

public partial class DoorObject : GridObject
{
	protected override string ModelName => "models/maya_testcube_100.vmdl";

	[Net] public bool Lockable { get; set; } = true;
	[Net] public bool Unlocked { get; private set; }
	[Net] public bool Used { get; private set; }

	public override void Spawn()
	{
		base.Spawn();

		Scale = 0.1f;
	}

	public override bool Use( GridEntity user, GridItem? usedItem, out bool itemUsed )
	{
		itemUsed = false;
		if ( usedItem is null && Unlocked )
		{
			Used = true;
			GridMap.End();
			return true;
		}

		if ( Lockable && usedItem is not KeyItem )
			return false;

		Unlocked = !Unlocked;
		return true;
	}

	public override bool IsObstructing()
	{
		return !Unlocked;
	}

	public override void Reset()
	{
		base.Reset();

		Unlocked = false;
		Used = false;
	}

	public override void DebugDraw()
	{
		base.DebugDraw();
		
		DebugOverlay.Text( $"Door(Open: {Unlocked})", Position, Color.White, 0.1f );
	}
}
