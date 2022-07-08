using CodeItOut.Grid;
using CodeItOut.Grid.Traverser;
using Sandbox;

namespace CodeItOut.Items;

public class TraverserItem : ModelEntity
{
	public virtual string ItemName => "";
	protected virtual string ModelPath => "";

	private GridTraverser _svHolder;
	private GridCell _svCurrentGridCell;

	public override void Spawn()
	{
		base.Spawn();
		
		SetModel( ModelPath );
	}

	public virtual void OnPickup( GridTraverser holder )
	{
		Host.AssertServer();

		_svHolder = holder;
		Owner = holder;
		
		EnableDrawing = false;
		holder.Items.Add( this );

		if ( _svCurrentGridCell is not null )
			_svCurrentGridCell.GroundItem = null;
	}

	public virtual void OnDrop( GridCell droppedGridCell )
	{
		Host.AssertServer();

		_svCurrentGridCell = droppedGridCell;
		Owner = droppedGridCell.Grid;
		
		droppedGridCell.GroundItem = this;
		Position = droppedGridCell.WorldPositionCenter;
		EnableDrawing = true;

		if ( _svHolder is null )
			return;

		_svHolder.Items.Remove( this );
		_svHolder = null;
	}

	public virtual void DebugDraw()
	{
	}
}
