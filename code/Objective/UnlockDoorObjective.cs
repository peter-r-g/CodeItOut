using CodeItOut.Grid;
using Sandbox;

namespace CodeItOut.Objective;

public partial class UnlockDoorObjective : BaseObjective
{
	[Net] public DoorObject Door { get; set; }
	
	public override bool IsCompleted()
	{
		return Door.Unlocked;
	}
}
