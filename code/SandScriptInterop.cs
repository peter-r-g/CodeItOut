using System;
using CodeItOut.Grid;
using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using Sandbox;
using SandScript;

namespace CodeItOut;

public static class SandScriptInterop
{
	#region Gameplay Methods
	
	public class Gameplay
	{
		[ScriptMethod( "TurnLeft" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void TurnLeft( Script script )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.TurnLeft );
		}

		[ScriptMethod( "TurnRight" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void TurnRight( Script script )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.TurnRight );
		}

		[ScriptMethod( "MoveForward" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void MoveForward( Script script )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.MoveForward );
		}

		[ScriptMethod( "UseObject" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void UseObject( Script script )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.UseObject );
		}

		[ScriptMethod( "UseItem" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "indexToUse", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void UseItem( Script script, double indexToUse )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.UseItem, (int)indexToUse );
		}

		[ScriptMethod( "PickupItem" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "indexToPlaceIn", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void PickupItem( Script script, double indexToPlaceIn )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.PickupItem, (int)indexToPlaceIn );
		}

		[ScriptMethod( "DropItem" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "indexToDrop", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void DropItem( Script script, double indexToDrop )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.DropItem, (int)indexToDrop );
		}

		[ScriptMethod( "Wait" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void Wait( Script script )
		{
			if ( ConsoleSystem.Caller is null || ConsoleSystem.Caller.Pawn is not Pawn pawn )
				return;

			pawn.Map.Traverser.AddAction( TraverserAction.Wait );
		}
	}
	
	#endregion

	#region Map Making

	public class MapMaking
	{
		public static GridBuilder MapBuilder = new();

		[ScriptMethod( "SetSize" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void SetSize( Script script, double x, double y )
		{
			MapBuilder.WithSize( (int)x, (int)y );
		}

		[ScriptMethod( "SetCellSize" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void SetCellSize( Script script, double x, double y )
		{
			MapBuilder.WithCellSize( (int)x, (int)y );
		}
		
		[ScriptMethod( "SetStartPosition" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void SetStartPosition( Script script, double x, double y )
		{
			MapBuilder.WithStartPosition( (int)x, (int)y );
		}

		[ScriptMethod( "PlaceFloor" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void PlaceFloor( Script script, double x, double y )
		{
			MapBuilder.AddObject( (int)x, (int)y, typeof(FloorObject) );
		}

		[ScriptMethod( "PlaceWall" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodParameter( 3, "direction", typeof(string) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void PlaceWall( Script script, double x, double y, string direction )
		{
			direction = direction.ToLower();
			if ( !Enum.TryParse<Direction>( direction, true, out var dir ) )
				throw new Exception();

			MapBuilder.AddObject( (int)x, (int)y, typeof(WallObject), dir );
		}

		[ScriptMethod( "PlaceKey" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void PlaceKey( Script script, double x, double y )
		{
			MapBuilder.AddItem( (int)x, (int)y, typeof(KeyItem) );
		}

		[ScriptMethod( "PlaceUnlockedExit" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodParameter( 3, "direction", typeof(string) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void PlaceUnlockedExit( Script script, double x, double y, string direction )
		{
			direction = direction.ToLower();
			if ( !Enum.TryParse<Direction>( direction, true, out var dir ) )
				throw new Exception();

			MapBuilder.AddObject( (int)x, (int)y, typeof(DoorObject), dir );
		}
		
		[ScriptMethod( "PlaceLockedExit" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "x", typeof(double) )]
		[ScriptMethodParameter( 2, "y", typeof(double) )]
		[ScriptMethodParameter( 3, "direction", typeof(string) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void PlaceLockedExit( Script script, double x, double y, string direction )
		{
			direction = direction.ToLower();
			if ( !Enum.TryParse<Direction>( direction, true, out var dir ) )
				throw new Exception();

			MapBuilder.AddObject( (int)x, (int)y, typeof(DoorObject), dir );
		}
	}
	
	#endregion
}
