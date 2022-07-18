using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SandScript;

public class VariableContainer<TKey, TValue> : IDictionary<TKey, TValue> where TKey : notnull
{
	public VariableContainer<TKey, TValue>? Parent { get; }
	public IReadOnlyDictionary<Guid, VariableContainer<TKey, TValue>> Children => _children;
	
	public Guid Guid { get; }

	public TValue this[ TKey key ]
	{
		get => _variables[key];
		set => _variables[key] = value;
	}

	public ICollection<TKey> Keys => _variables.Keys;
	public ICollection<TValue> Values => _variables.Values;
	
	public int Count => _variables.Count;
	public bool IsReadOnly => false;

	private readonly Dictionary<Guid, VariableContainer<TKey, TValue>> _children;
	private readonly Dictionary<TKey, TValue> _variables;
	private readonly IEqualityComparer<TKey>? _comparer;

	public VariableContainer( Guid guid, VariableContainer<TKey, TValue>? parent,
		IEnumerable<KeyValuePair<TKey, TValue>>? startVariables, IEqualityComparer<TKey>? comparer )
	{
		Parent = parent;
		Guid = guid;

		_comparer = comparer;
		_children = new Dictionary<Guid, VariableContainer<TKey, TValue>>();
		_variables = startVariables is not null
			? new Dictionary<TKey, TValue>( startVariables, comparer )
			: new Dictionary<TKey, TValue>( comparer );
	}

	public VariableContainer<TKey, TValue> AddChild( Guid guid, IEnumerable<KeyValuePair<TKey, TValue>>? startVariables )
	{
		var container = new VariableContainer<TKey, TValue>( guid, this, startVariables, _comparer );
		_children.Add( guid, container );
		return container;
	}
	
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	{
		return _variables.GetEnumerator();
	}

	public void Add( TKey key, TValue value )
	{
		_variables.Add( key, value );
	}

	public void Add( KeyValuePair<TKey, TValue> item )
	{
		Add( item.Key, item.Value );
	}

	public bool Remove( TKey key )
	{
		return _variables.Remove( key );
	}

	public bool Remove( KeyValuePair<TKey, TValue> item )
	{
		return _variables.Remove( item.Key );
	}

	public void Clear()
	{
		_variables.Clear();
	}

	public void CopyTo( KeyValuePair<TKey, TValue>[] array, int arrayIndex )
	{
		throw new NotImplementedException();
	}

	public bool Contains( KeyValuePair<TKey, TValue> item )
	{
		if ( TryGetValue( item.Key, out var value ) && value is not null )
			return value.Equals( item.Value );

		return false;
	}

	public bool ContainsKey( TKey key ) => ContainsKey( key, true, out _ );
	public bool ContainsKey( TKey key, bool recursive ) => ContainsKey( key, recursive, out _ );
	public bool ContainsKey( TKey key, bool recursive, [NotNullWhen( true )] out VariableContainer<TKey, TValue>? container )
	{
		if ( _variables.ContainsKey( key ) )
		{
			container = this;
			return true;
		}

		if ( recursive && Parent is not null )
			return Parent.ContainsKey( key, recursive, out container );

		container = default;
		return false;
	}

	public void AddOrUpdate( TKey key, TValue value )
	{
		if ( ContainsKey( key, true, out var container ) )
			container[key] = value;

		Add( key, value );
	}

	public bool TryGetValue( TKey key, [MaybeNullWhen( false )] out TValue value ) =>
		TryGetValue( key, out value, out _ );
	public bool TryGetValue( TKey key, [MaybeNullWhen( false )] out TValue value,
		[MaybeNullWhen( false )] out VariableContainer<TKey, TValue> container )
	{
		if ( !ContainsKey( key, true, out container ) )
		{
			value = default;
			return false;
		}

		value = container[key];
		return true;
	}
}
