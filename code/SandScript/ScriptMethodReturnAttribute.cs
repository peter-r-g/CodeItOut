using System;

namespace SandScript;

[AttributeUsage( AttributeTargets.Method )]
public sealed class ScriptMethodReturnAttribute : Attribute
{
	public readonly Type ReturnType;
	public readonly ITypeProvider ReturnTypeProvider;

	public ScriptMethodReturnAttribute( Type returnType )
	{
		ReturnType = returnType;
		ReturnTypeProvider = TypeProviders.GetByBackingType( returnType );
	}
}
