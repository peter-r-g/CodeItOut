using System;
using System.Collections.Generic;
using System.Reflection;
using Sandbox;

namespace SandScript;

public static class SandboxHelper
{
	public static object LatestReturnedValue
	{
		get
		{
			var value = _latestReturnedValue;
			_latestReturnedValue = null;
			return value;
		}
		set => _latestReturnedValue = value;
	}

	private static object _latestReturnedValue;
	
	public static List<ScriptMethodAttribute> GetNames( MethodDescription description )
	{
		var names = new List<ScriptMethodAttribute>();

		foreach ( var attribute in description.Attributes )
		{
			if ( attribute is ScriptMethodAttribute methodAttribute )
				names.Add( methodAttribute );
		}

		return names;
	}

	public static List<ScriptMethodParameterAttribute> GetParameters( MethodDescription description )
	{
		var parameters = new List<ScriptMethodParameterAttribute>();

		foreach ( var attribute in description.Attributes )
		{
			if ( attribute is ScriptMethodParameterAttribute parameterAttribute )
				parameters.Add( parameterAttribute );
		}

		return parameters;
	}

	public static ITypeProvider GetReturnTypeProvider( MethodDescription description ) =>
		GetReturnTypeProvider( description, out _ );
	public static ITypeProvider GetReturnTypeProvider( MethodDescription description, out Type type )
	{
		foreach ( var attribute in description.Attributes )
		{
			if ( attribute is not ScriptMethodReturnAttribute returnAttribute )
				continue;

			type = returnAttribute.ReturnType;
			return returnAttribute.ReturnTypeProvider;
		}

		type = TypeProviders.Builtin.Nothing.BackingType;
		return TypeProviders.Builtin.Nothing;
	}

	public static void Test( MethodDescription description )
	{
		List<ScriptMethodAttribute> methodAttributes = new();
		List<ScriptMethodParameterAttribute> parameterAttributes = new();
		ScriptMethodReturnAttribute returnAttribute = null;

		foreach ( var attribute in description.Attributes )
		{
			if ( attribute is ScriptMethodAttribute methodAttribute )
				methodAttributes.Add( methodAttribute );
			else if ( attribute is ScriptMethodParameterAttribute parameterAttribute )
				parameterAttributes.Add( parameterAttribute );
			else if ( attribute is ScriptMethodReturnAttribute methodReturnAttribute )
				returnAttribute = methodReturnAttribute;
		}

		foreach ( var methodAttribute in methodAttributes )
			Log.Info( $"Method: {methodAttribute.MethodName}" );
		
		foreach ( var parameterAttribute in parameterAttributes )
			Log.Info( $"Parameter: {parameterAttribute.ParameterIndex} - {parameterAttribute.ParameterName}, {parameterAttribute.ParameterTypeProvider}" );
		
		if ( returnAttribute is not null )
			Log.Info( $"Return: {returnAttribute.ReturnType}" );
	}
}
