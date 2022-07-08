using System;

namespace SandScript.Exceptions;

public class GlobalRedefinedException : Exception
{
	public readonly string VariableName;
	
	public GlobalRedefinedException( string varName )
		: base( "\"" + varName + "\" is already a defined global variable" )
	{
		VariableName = varName;
	}
}
