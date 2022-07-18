using System;
using SandScript.Exceptions;

namespace SandScript;

public sealed class ScriptValue
{
	public readonly Type Type;
	public readonly ITypeProvider TypeProvider;

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
