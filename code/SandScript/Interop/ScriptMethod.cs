using System;
using System.Collections.Generic;
using Sandbox;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public class ScriptMethod : IEquatable<ScriptMethod>
{
	public readonly string Name;
	public readonly ITypeProvider ReturnTypeProvider;
	public readonly IReadOnlyList<(string, ITypeProvider)> Parameters;
	public readonly MethodSignature Signature;
	
	private readonly bool _isCsMethod;

	private readonly MethodDescription _methodDescription;
	private readonly MethodDeclarationAst _methodDeclarationAst;
	
	public ScriptMethod( MethodDeclarationAst methodDeclarationAst )
	{
		Name = methodDeclarationAst.MethodName;
		ReturnTypeProvider = methodDeclarationAst.ReturnTypeAst.TypeProvider;

		_isCsMethod = false;
		_methodDeclarationAst = methodDeclarationAst;

		var parameters = new List<(string, ITypeProvider)>();
		foreach ( var parameter in methodDeclarationAst.ParameterAsts )
			parameters.Add( (parameter.ParameterNameAst.VariableName, parameter.ParameterTypeAst.TypeProvider) );
		Parameters = parameters;
		
		Signature = MethodSignature.From( this );
	}

	public ScriptMethod( MethodDescription description, ScriptMethodAttribute attribute )
	{
		Name = attribute.MethodName;
		ReturnTypeProvider = SandboxHelper.GetReturnTypeProvider( description );

		_isCsMethod = true;
		_methodDescription = description;

		var parameters = SandboxHelper.GetParameters( description );
		var formattedParameters = new List<(string, ITypeProvider)>();
		for ( var i = 1; i < parameters.Count; i++ )
		{
			var parameter = parameters[i];
			formattedParameters.Add( (parameter.ParameterName, parameter.ParameterTypeProvider) );
		}
		Parameters = formattedParameters;
		
		Signature = MethodSignature.From( this );
	}

	public object? Invoke( Interpreter interpreter, object?[] values )
	{
		if ( _isCsMethod )
		{
			var parameters = new object[values.Length + 1];
			Array.Copy( values, 0, parameters, 1, values.Length );
			
			parameters[0] = interpreter.Owner;
			var methodParameters = SandboxHelper.GetParameters( _methodDescription );
			for ( var i = 0; i < parameters.Length-1; i++ )
			{
				if ( methodParameters![i + 1].ParameterType == typeof(ScriptValue) )
					parameters[i + 1] = ScriptValue.From( values[i] );
			}
			
			var result = _methodDescription.InvokeWithReturn<object>( null, parameters );
			if ( result is ScriptValue sv )
				return sv.Value;
			
			return result;
		}
		else
		{
			var parameters = new Dictionary<string, object>();
			for ( var i = 0; i < values.Length; i++ )
				parameters.Add( Parameters[i].Item1, values[i] );
			
			using var scope = interpreter.Variables.Enter( Guid.Empty, parameters );
			var result = interpreter.Visit( _methodDeclarationAst!.BodyAst );

			interpreter.Returning = false;
			return result;
		}
	}

	public bool Equals( ScriptMethod? other )
	{
		if ( ReferenceEquals( null, other ) )
			return false;

		if ( ReferenceEquals( this, other ) )
			return true;

		return ReturnTypeProvider == other.ReturnTypeProvider && Signature.Equals( other.Signature );
	}

	public override bool Equals( object? obj )
	{
		if ( ReferenceEquals( null, obj ) )
			return false;

		if ( ReferenceEquals( this, obj ) )
			return true;

		return obj.GetType() == GetType() && Equals( (ScriptMethod)obj );
	}

	public override int GetHashCode()
	{
		return Signature.GetHashCode();
	}
}
