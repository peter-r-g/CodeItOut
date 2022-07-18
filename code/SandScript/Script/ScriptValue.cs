using System;
using SandScript.Exceptions;

namespace SandScript;

/// <summary>
/// The container for all values in SandScript. This is only used to pass values to the end developer.
/// </summary>
public sealed class ScriptValue
{
	/// <summary>
	/// The CSharp type of the value.
	/// </summary>
	public readonly Type Type;
	/// <summary>
	/// The SandScript type of the value.
	/// </summary>
	public readonly ITypeProvider TypeProvider;

	/// <summary>
	/// The getter for the value contained.
	/// </summary>
	public object? Value
	{
		get
		{
			if ( _value is ScriptVariable variable )
				return variable.GetValue();
			
			return _value;
		}
	}

	private readonly object? _value;

	public ScriptValue( object? value )
	{
		if ( value is ScriptVariable variable )
		{
			Type = variable.TypeProvider.BackingType;
			TypeProvider = variable.TypeProvider;
		}
		else
		{
			Type = value is null ? TypeProviders.Builtin.Nothing.BackingType : value.GetType();
			TypeProvider = TypeProviders.GetByBackingType( Type )!;
		}
		
		_value = value;
	}

	public bool Equals( ScriptValue other )
	{
		return Type == other.Type && TypeProvider.Compare( Value, other.Value );
	}

	public override bool Equals( object? obj )
	{
		if ( ReferenceEquals( null, obj ) )
			return false;

		if ( ReferenceEquals( this, obj ) )
			return true;

		return obj.GetType() == GetType() && Equals( (ScriptValue)obj );
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( Type, Value );
	}

	public override string ToString()
	{
		return nameof(ScriptValue) + "( Type: " + Type + ", Value: " + Value + " )";
	}

	/// <summary>
	/// Creates a new instance of <see cref="ScriptValue"/> from the value passed.
	/// </summary>
	/// <param name="value">The value to create a <see cref="ScriptValue"/> from.</param>
	/// <typeparam name="T">The type of value.</typeparam>
	/// <returns>The new instance of <see cref="ScriptValue"/> created.</returns>
	/// <exception cref="UnsupportedException">Thrown when the type of the value passed is not supported.</exception>
	public static ScriptValue From<T>( T value )
	{
		switch ( value )
		{
			case null:
			case ScriptVariable:
				return new ScriptValue( value );
			case ScriptValue sv:
				return sv;
		}
		
		var provider = TypeProviders.GetByValue( value );
		if ( provider is not null )
			return new ScriptValue( value );
		
		throw new UnsupportedException();
	}
}
