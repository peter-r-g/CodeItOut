using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using CodeItOut.Utility;
using Sandbox;

namespace CodeItOut.Grid;

public partial class GridObject : ModelEntity
{
	public virtual string ModelName => "";
	
	[Net] public Direction Direction { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		SetModel( ModelName );
	}

	public virtual bool Use( GridTraverser user, TraverserItem usedItem )
	{
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
