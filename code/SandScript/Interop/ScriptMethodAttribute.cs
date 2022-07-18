using System;

namespace SandScript;

/// <summary>
/// Marks a method for being usable by SandScript.
/// </summary>
[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class ScriptMethodAttribute : Attribute
{
	/// <summary>
	/// The name the method should have in SandScript.
	/// </summary>
	public readonly string MethodName;

	public ScriptMethodAttribute( string methodName )
	{
		MethodName = methodName;
	}
}
