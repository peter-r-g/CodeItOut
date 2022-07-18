namespace CodeItOut.Grid;

public class FloorObject : GridObject
{
	protected override string ModelName => "models/maya_testcube_100.vmdl";
	public override Vector3 Offset => new(0, 0, -100);
}
