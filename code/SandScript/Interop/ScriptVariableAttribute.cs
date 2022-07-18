using System;

namespace SandScript;

/// <summary>
/// Marks a variable for being usable by SandScript.
/// </summary>
[AttributeUsage( AttributeTargets.Property, AllowMultiple = true )]
public class ScriptVariableAttribute : Attribute
{
	/// <summary>
	/// The name the property should have in SandScript.
	/// </summary>
	public readonly string VariableName;
	/// <summary>
	/// Whether or not the variable can be read from. NOTE: This is independent of whether the variable has a getter.
	/// </summary>
	public bool CanRead { get; set; } = true;
	/// <summary>
	/// Whether or not the variable can be written to. NOTE: This is independent of whether the variable has a setter.
	/// </summary>
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
