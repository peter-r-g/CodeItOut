using System;

namespace SandScript;

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public sealed class ScriptMethodParameterAttribute : Attribute
{
	public readonly int ParameterIndex;
	public readonly string ParameterName;
	public readonly Type ParameterType;
	public readonly ITypeProvider ParameterTypeProvider;

	public ScriptMethodParameterAttribute( int parameterIndex, string parameterName, Type inputType )
	{
		ParameterIndex = parameterIndex;
		ParameterName = parameterName;
		ParameterType = inputType;
		ParameterTypeProvider = TypeProviders.GetByBackingType( inputType );
	}
}
