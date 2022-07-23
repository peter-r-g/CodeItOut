using System.Text.Json.Serialization;
using CodeItOut.Grid.Traverser;

namespace CodeItOut.Grid;

public class TraverserAction
{
	// TODO: I hate that any of this has to exist. ServerRpc my beloved when :(
	[JsonInclude]
	[JsonPropertyName( "a" )]
	public TraverserActionType ActionType;
	[JsonInclude]
	[JsonPropertyName( "b" )]
	public object[] ActionArguments;

	public TraverserAction()
	{
	}

	public TraverserAction( TraverserActionType actionType, params object[] arguments )
	{
		ActionType = actionType;
		ActionArguments = arguments;
	}
}
