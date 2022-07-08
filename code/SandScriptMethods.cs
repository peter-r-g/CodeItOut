using CodeItOut.Grid.Traverser;
using Sandbox;
using SandScript;

namespace CodeItOut;

public class SandScriptMethods
{
	[ScriptMethod( "TurnLeft" )]
	[ScriptMethodParameter( 0, "script", typeof(Script) )]
	[ScriptMethodReturn( typeof(void) )]
	public static void TurnLeft( Script script )
	{
		if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
			return;
		
		pawn.Grid.Traverser.AddAction( TraverserAction.TurnLeft );
	}

	[ScriptMethod( "TurnRight" )]
	[ScriptMethodParameter( 0, "script", typeof(Script) )]
	[ScriptMethodReturn( typeof(void) )]
	public static void TurnRight( Script script )
	{
		if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
			return;
		
		pawn.Grid.Traverser.AddAction( TraverserAction.TurnRight );
	}

	[ScriptMethod( "MoveForward" )]
	[ScriptMethodParameter( 0, "script", typeof(Script) )]
	[ScriptMethodReturn( typeof(void) )]
	public static void MoveForward( Script script )
	{
		if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
			return;
		
		pawn.Grid.Traverser.AddAction( TraverserAction.MoveForward );
	}

	[ScriptMethod( "UseItem" )]
	[ScriptMethodParameter( 0, "script", typeof(Script) )]
	[ScriptMethodParameter( 1, "itemIndex", typeof(double) )]
	[ScriptMethodReturn( typeof(void) )]
	public static void UseItem( Script script, double itemIndex )
	{
		if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
			return;
		
		pawn.Grid.Traverser.AddAction( TraverserAction.UseItem, itemIndex );
	}

	[ScriptMethod( "PickupItem" )]
	[ScriptMethodParameter( 0, "script", typeof(Script) )]
	[ScriptMethodReturn( typeof(void) )]
	public static void PickupItem( Script script )
	{
		if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
			return;

		pawn.Grid.Traverser.AddAction( TraverserAction.PickupItem );
	}

	[ScriptMethod( "DropItem" )]
	[ScriptMethodParameter( 0, "script", typeof(Script) )]
	[ScriptMethodParameter( 1, "itemIndex", typeof(double) )]
	[ScriptMethodReturn( typeof(void) )]
	public static void DropItem( Script script, double itemIndex )
	{
		if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
			return;

		pawn.Grid.Traverser.AddAction( TraverserAction.DropItem, itemIndex );
	}
}
