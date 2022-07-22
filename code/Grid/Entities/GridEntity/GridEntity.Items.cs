using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CodeItOut.Items;
using Sandbox;

namespace CodeItOut.Grid;

public partial class GridEntity
{
	[Net] public IList<GridItem> Items { get; private set; }
	[Net] public int ItemCapacity { get; set; }

	protected bool TryUseObject( out GridItem? usedItem, out bool itemUsed, int? itemIndexToUse = null )
	{
		usedItem = null;
		itemUsed = false;

		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return false;

		if ( itemIndexToUse is not null )
		{
			if ( Items.Count < itemIndexToUse )
				return false;
			
			usedItem = Items[itemIndexToUse.Value];
		}
		
		foreach ( var obj in cellInfo.GetObjectsInDirection( Direction ) )
		{
			if ( obj.Use( this, usedItem, out itemUsed ) )
				return true;
		}

		return false;
	}

	protected bool TryUseItem( int indexToUse, [NotNullWhen( true )] out GridItem? usedItem, out bool itemUsed )
	{
		usedItem = null;
		itemUsed = false;
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out _ ) )
			return false;

		if ( Items.Count < indexToUse )
			return false;
		
		usedItem = Items[indexToUse];
		itemUsed = usedItem.Use( this );

		return false;
	}
	
	protected bool TryPickupItem( int indexToPlaceIn, [NotNullWhen( true )] out GridItem? item )
	{
		item = null;
		if ( indexToPlaceIn > ItemCapacity - 1 )
			return false;
		
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return false;

		if ( cellInfo.GroundItem is null )
			return false;

		item = cellInfo.GroundItem;
		cellInfo.GroundItem.OnPickup( this );
		Items.Insert( indexToPlaceIn, item );
		return true;
	}

	protected bool TryDropItem( int indexToDrop, [NotNullWhen( true )] out GridItem? item )
	{
		item = null;
		if ( !GridMap.TryGetCellAt( GridPosition.X, GridPosition.Y, out var cellInfo ) )
			return false;

		if ( cellInfo.GroundItem is not null )
			return false;

		if ( Items.Count < indexToDrop )
			return false;

		item = Items[indexToDrop];
		item.OnDrop( cellInfo );
		Items.RemoveAt( indexToDrop );
		return true;
	}
}
