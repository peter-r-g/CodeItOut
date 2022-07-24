using System.Collections.Generic;
using CodeItOut.Grid;
using Sandbox;

namespace CodeItOut.Objective;

public abstract partial class BaseObjective : BaseNetworkable
{
	public GridMap? GridMap
	{
		get => Map;
		set
		{
			var oldGridMap = Map;
			Map = value;
			OnGridMapChanged( oldGridMap, value );
		}
	}
	[Net] private GridMap? Map { get; set; }
	
	[Net] public IList<BaseObjective> ChildObjectives { get; private set; }
	
	protected virtual void OnGridMapChanged( GridMap? oldGridMap, GridMap? newGridMap )
	{
	}
	
	public abstract bool IsCompleted();
}
