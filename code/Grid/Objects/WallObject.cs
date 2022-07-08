﻿namespace CodeItOut.Grid;

public class WallObject : GridObject
{
	public override string ModelName => "models/maya_testcube_100.vmdl";

	public override void Spawn()
	{
		base.Spawn();

		RenderColor = Color.Black;
		Scale = 0.1f;
	}

	public override bool IsObstructing()
	{
		return true;
	}

	public override void DebugDraw()
	{
		base.DebugDraw();
		
		DebugOverlay.Text( "Wall", Position, Color.White, 0.1f );
	}
}
