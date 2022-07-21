using Sandbox;

namespace CodeItOut.Grid;

public static class GridEvent
{
	#region MapWon
	
	public const string MapWonEvent = "codeitout.MapWon";
	
	public static class MapWon
	{
		public const string ServerEvent = MapWonEvent + ".Server";
		public const string ClientEvent = MapWonEvent + ".Client";
		
		public class ServerAttribute : EventAttribute
		{
			public ServerAttribute() : base( ServerEvent ) { }
		}
		
		public class ClientAttribute : EventAttribute
		{
			public ClientAttribute() : base( ClientEvent ) { }
		}
	}
	
	public class MapWonAttribute : EventAttribute
	{
		public MapWonAttribute() : base( MapWonEvent ) { }
	}
	
	#endregion

	#region MapLost

	public const string MapLostEvent = "codeitout.MapLost";
	
	public static class MapLost
	{
		public const string ServerEvent = MapLostEvent + ".Server";
		public const string ClientEvent = MapLostEvent + ".Client";
		
		public class ServerAttribute : EventAttribute
		{
			public ServerAttribute() : base( ServerEvent ) { }
		}
		
		public class ClientAttribute : EventAttribute
		{
			public ClientAttribute() : base( ClientEvent ) { }
		}
	}
	
	public class MapLostAttribute : EventAttribute
	{
		public MapLostAttribute() : base( MapLostEvent ) { }
	}
	
	#endregion
}
