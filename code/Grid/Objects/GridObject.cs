using CodeItOut.Items;
using Sandbox;

namespace CodeItOut.Grid;

public partial class GridObject : ModelEntity
{
	protected virtual string ModelName => "";
	public virtual Vector3 Offset => Vector3.Zero;

	[Net] public GridMap GridMap { get; set; }
	[Net] public GridCell Cell { get; set; }
	[Net] public Direction Direction { get; set; }

	public override void Spawn()
	{
		base.Spawn();
		SetModel( ModelName );
	}

	public virtual bool Use( GridEntity user, GridItem usedItem, out bool itemUsed )
	{
		itemUsed = false;
		return false;
	}

	public virtual bool IsObstructing()
	{
		return false;
	}

	public virtual void Reset()
	{
	}

	public virtual void DebugDraw()
	{
	}
}
