using System;
using System.Collections.Immutable;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public class MethodSignature : IEquatable<MethodSignature>
{
	private readonly string _name;
	private readonly ImmutableArray<ITypeProvider> _types;

	private MethodSignature( string name, ImmutableArray<ITypeProvider> types )
	{
		_name = name;
		_types = types;
	}
	
	private string StringifyTypes()
	{
		switch ( _types.Length )
		{
			case 0:
				return string.Empty;
			case 1:
				return _types[0].ToString()!;
		}

		var typeSignature = _types[0].ToString();
		for ( var i = 1; i < _types.Length; i++ )
		{
			typeSignature += ',';
			typeSignature += _types[i];
		}

		return typeSignature!;
	}

	public override string ToString()
	{
		return _name + '(' + StringifyTypes() + ')';
	}

	public bool Equals( MethodSignature? other )
	{
		if ( other is null )
			return false;
		
		if ( _name != other._name )
			return false;

		if ( _types.Length != other._types.Length )
			return false;

		for ( var i = 0; i < _types.Length; i++ )
		{
			if ( _types[i] == other._types[i] )
				continue;

			if ( _types[i] == TypeProviders.Builtin.Variable ||
			     other._types[i] == TypeProviders.Builtin.Variable )
				continue;
			
			return false;
		}

		return true;
	}

	public override bool Equals( object? obj )
	{
		if ( ReferenceEquals( null, obj ) )
			return false;

		if ( ReferenceEquals( this, obj ) )
			return true;

		return obj.GetType() == GetType() && Equals( (MethodSignature)obj );
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( _name, _types );
	}

	public static MethodSignature From( string methodName, ScriptMethod method )
	{
		var parameterTypes = ImmutableArray.CreateBuilder<ITypeProvider>();
		foreach ( var parameter in method.Parameters )
			parameterTypes.Add( parameter.Item2 );

		return new MethodSignature( methodName, parameterTypes.ToImmutable() );
	}

	public static MethodSignature From( ScriptMethod method )
	{
		return From( method.Name, method );
	}

	public static MethodSignature From( MethodDeclarationAst methodDeclaration )
	{
		var parameterTypes = ImmutableArray.CreateBuilder<ITypeProvider>();
		foreach ( var parameter in methodDeclaration.ParameterAsts )
			parameterTypes.Add( parameter.ParameterType );

		return new MethodSignature( methodDeclaration.MethodName, parameterTypes.ToImmutable() );
	}

	public static MethodSignature From( MethodCallAst methodCall )
	{
		return new MethodSignature( methodCall.MethodName, methodCall.ArgumentTypes );
	}
}
