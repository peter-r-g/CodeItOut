using System;

namespace SandScript;

[AttributeUsage( AttributeTargets.Property, AllowMultiple = true )]
public class ScriptVariableAttribute : Attribute
{
	public readonly string VariableName;
	public bool CanRead { get; set; } = true;
	public bool CanWrite { get; set; } = true;
	
	public ScriptVariableAttribute( string variableName )
	{
		VariableName = variableName;
	}

	public ScriptVariableAttribute( string variableName, bool canRead, bool canWrite ) : this( variableName )
	{
		CanRead = canRead;
		CanWrite = canWrite;
	}
}
