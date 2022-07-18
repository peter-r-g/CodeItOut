using System;
using System.Collections.Generic;

namespace SandScript;

public static class TypeProviders
{
	private static readonly List<ITypeProvider> Types = new()
	{
		new NothingTypeProvider(),
		new VariableTypeProvider(),
		new BooleanTypeProvider(),
		new CharacterTypeProvider(),
		new NumberTypeProvider(),
		new MethodTypeProvider(),
		new StringTypeProvider()
	};

	public static void Register<T>( T provider ) where T : ITypeProvider
	{
		Types.Add( provider );
	}

	public static IEnumerable<ITypeProvider> GetAll()
	{
		return Types;
	}

	public static ITypeProvider? Get<T>() where T : ITypeProvider
	{
		foreach ( var provider in Types )
		{
			if ( provider is T )
				return provider;
		}

		return null;
	}

	public static ITypeProvider? GetByName( string name )
	{
		foreach ( var type in Types )
		{
			if ( type.TypeName == name )
				return type;
		}

		return null;
	}

	public static ITypeProvider? GetByIdentifier( string identifier )
	{
		foreach ( var type in Types )
		{
			if ( type.TypeIdentifier == identifier )
				return type;
		}

		return null;
	}

	public static ITypeProvider? GetByBackingType( Type backingType )
	{
		if ( backingType == typeof(ScriptValue) )
			return Builtin.Variable;

		foreach ( var typeProvider in Types )
		{
			if ( typeProvider.BackingType == backingType )
				return typeProvider;
		}

		return null;
	}

	public static ITypeProvider? GetByValue( object? value )
	{
		return value is null ? Builtin.Nothing : GetByBackingType( value.GetType() );
	}

	public static class Builtin
	{
		public static readonly ITypeProvider Nothing = Get<NothingTypeProvider>()!;
		public static readonly ITypeProvider Variable = Get<VariableTypeProvider>()!;
		public static readonly ITypeProvider Boolean = Get<BooleanTypeProvider>()!;
		public static readonly ITypeProvider Character = Get<CharacterTypeProvider>()!;
		public static readonly ITypeProvider Method = Get<MethodTypeProvider>()!;
		public static readonly ITypeProvider Number = Get<NumberTypeProvider>()!;
		public static readonly ITypeProvider String = Get<StringTypeProvider>()!;
	}
}
