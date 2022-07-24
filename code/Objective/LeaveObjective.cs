using System.Linq;
using CodeItOut.Grid;
using Sandbox;

namespace CodeItOut.Objective;

public partial class LeaveObjective : BaseObjective
{
	[Net] public DoorObject Exit { get; set; }

	protected override void OnGridMapChanged( GridMap? oldGridMap, GridMap? newGridMap )
	{
		base.OnGridMapChanged( oldGridMap, newGridMap );

		if ( oldGridMap is not null )
			oldGridMap.MapReady -= MapReady;

		if ( newGridMap is not null )
			newGridMap.MapReady += MapReady;
	}
	
	private void MapReady()
	{
		if ( !Host.IsServer )
			return;
		
		ChildObjectives.Clear();
		Exit = GridMap!.GetObjectsOfType<DoorObject>().First();
		if ( Exit.Lockable )
			ChildObjectives.Add( new UnlockDoorObjective {Door = Exit} );
	}

	public override bool IsCompleted()
	{
		return Exit.Used;
	}
}
