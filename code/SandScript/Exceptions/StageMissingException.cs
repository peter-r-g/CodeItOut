using System;

namespace SandScript.Exceptions;

public sealed class StageMissingException : Exception
{
	public readonly Type StageType;

	public StageMissingException( Type stageType ) : base( "The stage type " + stageType + " is missing" )
	{
		StageType = stageType;
	}
}
