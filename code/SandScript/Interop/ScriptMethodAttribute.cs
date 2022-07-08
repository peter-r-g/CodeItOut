using System;

namespace SandScript;

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class ScriptMethodAttribute : Attribute
{
	public readonly string MethodName;

	public ScriptMethodAttribute( string methodName )
	{
		MethodName = methodName;
	}
}
