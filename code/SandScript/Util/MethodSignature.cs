using System;
using System.Collections.Immutable;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

/// <summary>
/// Represents a method signature. Used to grab the correct method overload.
/// </summary>
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

	/// <summary>
	/// Creates a new method signature from a method and a different name.
	/// </summary>
	/// <param name="methodName">The new name of the method.</param>
	/// <param name="method">The method to create a signature from.</param>
	/// <returns>A new instance of <see cref="MethodSignature"/> built from the new name and method.</returns>
	public static MethodSignature From( string methodName, ScriptMethod method )
	{
		var parameterTypes = ImmutableArray.CreateBuilder<ITypeProvider>();
		foreach ( var parameter in method.Parameters )
			parameterTypes.Add( parameter.Item2 );

		return new MethodSignature( methodName, parameterTypes.ToImmutable() );
	}

	/// <summary>
	/// Creates a new method signature from a method.
	/// </summary>
	/// <param name="method">The method to create a signature from.</param>
	/// <returns>A new instance of <see cref="MethodSignature"/> built from the method passed.</returns>
	public static MethodSignature From( ScriptMethod method )
	{
		return From( method.Name, method );
	}

	/// <summary>
	/// Creates a new method signature from a method declaration.
	/// </summary>
	/// <param name="methodDeclaration">The method declaration to create a signature from.</param>
	/// <returns>A new instance of <see cref="MethodSignature"/> built from the method declaration.</returns>
	public static MethodSignature From( MethodDeclarationAst methodDeclaration )
	{
		var parameterTypes = ImmutableArray.CreateBuilder<ITypeProvider>();
		foreach ( var parameter in methodDeclaration.ParameterAsts )
			parameterTypes.Add( parameter.ParameterType );

		return new MethodSignature( methodDeclaration.MethodName, parameterTypes.ToImmutable() );
	}

	/// <summary>
	/// Creates a new method signature from a method call.
	/// </summary>
	/// <param name="methodCall">The method call to create a signature from/</param>
	/// <returns>A new instance of <see cref="MethodSignature"/> built from the method call.</returns>
	public static MethodSignature From( MethodCallAst methodCall )
	{
		return new MethodSignature( methodCall.MethodName, methodCall.ArgumentTypes );
	}
}
