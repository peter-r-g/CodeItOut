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
	public int ActionArgument;

	public TraverserAction()
	{
	}

	public TraverserAction( TraverserActionType actionType, int argument = -1 )
	{
		ActionType = actionType;
		ActionArgument = argument;
	}
}
