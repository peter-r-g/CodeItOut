using System;
using System.Collections.Generic;
using CodeItOut.Grid;
using CodeItOut.Grid.Traverser;
using CodeItOut.Items;
using SandScript;

namespace CodeItOut;

public static class SandScriptInterop
{
	#region Gameplay Methods
	
	public class Gameplay
	{
		public static readonly List<TraverserAction> ClActions = new();
		
		[ScriptMethod( "TurnLeft" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void TurnLeft( Script script )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.TurnLeft ) );
		}

		[ScriptMethod( "TurnRight" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void TurnRight( Script script )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.TurnRight ) );
		}

		[ScriptMethod( "MoveForward" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void MoveForward( Script script )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.MoveForward ) );
		}

		[ScriptMethod( "UseObject" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void UseObject( Script script )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.UseObject ) );
		}

		[ScriptMethod( "UseObject" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "itemIndexToUse", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void UseObject( Script script, double itemIndexToUse )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.UseObject, (int)itemIndexToUse ) );	
		}

		[ScriptMethod( "UseItem" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "indexToUse", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void UseItem( Script script, double indexToUse )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.UseItem, (int)indexToUse ) );
		}

		[ScriptMethod( "PickupItem" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "indexToPlaceIn", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void PickupItem( Script script, double indexToPlaceIn )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.PickupItem, (int)indexToPlaceIn ) );
		}

		[ScriptMethod( "DropItem" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "indexToDrop", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void DropItem( Script script, double indexToDrop )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.DropItem, (int)indexToDrop ) );
		}

		[ScriptMethod( "ThrowItem" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "indexToThrow", typeof(double) )]
		[ScriptMethodParameter( 2, "xDelta", typeof(double) )]
		[ScriptMethodParameter( 3, "yDelta", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void ThrowItem( Script script, double indexToThrow, double xDelta, double yDelta )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.ThrowItem, (int)indexToThrow, (int)xDelta,
				(int)yDelta ) );
		}

		[ScriptMethod( "Wait" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void Wait( Script script )
		{
			ClActions.Add( new TraverserAction( TraverserActionType.Wait ) );
		}
	}
	
	#endregion

	#region Map Making Methods

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

		[ScriptMethod( "SetItemCap" )]
		[ScriptMethodParameter( 0, "script", typeof(Script) )]
		[ScriptMethodParameter( 1, "itemCap", typeof(double) )]
		[ScriptMethodReturn( typeof(void) )]
		public static void SetItemCap( Script script, double itemCap )
		{
			MapBuilder.WithItemCap( (int)itemCap );
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
