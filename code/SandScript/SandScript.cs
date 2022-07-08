using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SandScript.Exceptions;

namespace SandScript;

public static class SandScript
{
	public static IEnumerable<ScriptMethod> CustomMethods => CustomMethodCache;
	private static readonly List<ScriptMethod> CustomMethodCache = new();
	public static IEnumerable<ScriptVariable> CustomVariables => CustomVariableCache;
	private static readonly List<ScriptVariable> CustomVariableCache = new();

	public static void RegisterClassMethods()
	{
		var methods = TypeLibrary.FindStaticMethods<ScriptMethodAttribute>();

		foreach ( var method in methods )
		{
			var returnTypeProvider = SandboxHelper.GetReturnTypeProvider( method, out var returnType );
			if ( returnTypeProvider is null )
				throw new ReturnTypeUnsupportedException( returnType );
			
			var parameters = SandboxHelper.GetParameters( method );
			if ( parameters.Count == 0 || parameters[0].ParameterType != typeof(Script) )
				throw new ParameterException( "First parameter must be of type " + nameof(Script) + "." );

			for ( var i = 1; i < parameters.Count; i++ )
			{
				var parameter = parameters[i];
				if ( parameter.ParameterType != typeof(ScriptValue) && parameter.ParameterTypeProvider is null )
					throw new ParameterException( "Parameter type \"" + parameter.ParameterType + "\" is unsupported." );
			}

			var methodNameAttributes = SandboxHelper.GetNames( method );
			foreach ( var methodNameAttribute in methodNameAttributes )
				CustomMethodCache.Add( new ScriptMethod( method, methodNameAttribute ) );
		}
	}

	public static void RegisterClassVariables<T>() where T : class
	{
		var properties = typeof(T).GetProperties( BindingFlags.Public | BindingFlags.Static )
			.Where( p => p.GetCustomAttributes( typeof(ScriptVariableAttribute), false ).Length > 0 );

		foreach ( var property in properties )
		{
			if ( TypeProviders.GetByBackingType( property.PropertyType ) is null )
				throw new TypeUnsupportedException( property.PropertyType );

			foreach ( var attribute in property.GetCustomAttributes<ScriptVariableAttribute>() )
			{
				if ( attribute.CanRead && !property.CanRead )
					throw new UnreadableVariableException( property, attribute );

				if ( attribute.CanWrite && !property.CanWrite )
					throw new UnwritableVariableException( property, attribute );
				
				CustomVariableCache.Add( new ScriptVariable( property, attribute ) );
			}
		}
	}
}
