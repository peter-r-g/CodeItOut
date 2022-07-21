using CodeItOut.Grid;
using CodeItOut.Grid.Traverser;
using Sandbox;

namespace CodeItOut.Items;

public class GridItem : ModelEntity
{
	
	public virtual string ItemName => "";
	protected virtual string ModelPath => "";

	private GridEntity _svHolder;
	private GridCell _svCurrentGridCell;

	public override void Spawn()
	{
		base.Spawn();
		
		SetModel( ModelPath );
	}

	public virtual void OnPickup( GridEntity holder )
	{
		Host.AssertServer();

		_svHolder = holder;
		Owner = holder;
		
		EnableDrawing = false;

		if ( _svCurrentGridCell is not null )
			_svCurrentGridCell.GroundItem = null;
	}

	public virtual void OnDrop( GridCell droppedGridCell )
	{
		Host.AssertServer();

		_svCurrentGridCell = droppedGridCell;
		Owner = droppedGridCell.GridMap;
		
		droppedGridCell.GroundItem = this;
		Position = droppedGridCell.WorldPositionCenter;
		EnableDrawing = true;

		if ( _svHolder is null )
			return;

		_svHolder = null;
	}

	public virtual void DebugDraw()
	{
	}
}
