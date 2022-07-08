using Sandbox;

namespace CodeItOut.Utility;

public partial class VectorLookAtCamera : LookAtCamera
{
	/// <summary>
	/// Vector to look at
	/// </summary>
	[Net] public Vector3 TargetVector { get; set; }

	protected override Vector3 GetTargetPos()
	{
		return TargetVector;
	}
}
