﻿using System;
using System.Collections.Generic;
using CodeItOut.Items;
using CodeItOut.Utility;

namespace CodeItOut.Grid;

public class ActionResult
{
	public readonly ActionState ActionState;
	public readonly IntVector2 GridPosition;
	public readonly Direction Direction;
	public readonly IEnumerable<GridItem> GainedItems;
	public readonly IEnumerable<GridItem> LostItems;

	private ActionResult( ActionState actionState, IntVector2 gridPosition, Direction direction,
		IEnumerable<GridItem> gainedItems, IEnumerable<GridItem> lostItems )
	{
		ActionState = actionState;
		GridPosition = gridPosition;
		Direction = direction;
		GainedItems = gainedItems;
		LostItems = lostItems;
	}

	public static ActionResult Success( IntVector2 gridPosition, Direction direction, IEnumerable<GridItem> gainedItems, IEnumerable<GridItem> lostItems )
	{
		return new ActionResult( ActionState.Succeeded, gridPosition, direction, gainedItems, lostItems );
	}

	public static ActionResult Fail()
	{
		return new ActionResult( ActionState.Failed, IntVector2.Zero, Direction.None, ArraySegment<GridItem>.Empty,
			ArraySegment<GridItem>.Empty );
	}
}
