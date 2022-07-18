using System;
using System.Collections.Generic;

namespace SandScript;

public class VariableManager<TKey, TValue> where TKey : notnull
{
	public VariableContainer<TKey, TValue> Root
	{
		get
		{
			var root = Current;
			while ( root.Parent is not null )
				root = root.Parent;
			
			return root;
		}
	}

	public VariableContainer<TKey, TValue> Current;

	public VariableManager( IEqualityComparer<TKey>? comparer )
	{
		Current = new VariableContainer<TKey, TValue>( Guid.Empty, null, null, comparer );
	}

	public ChildHandle Enter( Guid guid, IEnumerable<KeyValuePair<TKey, TValue>>? startVariables = null )
	{
		var handle = new ChildHandle( this );
		if ( !Current.Children.ContainsKey( guid ) )
		{
			Current = Current.AddChild( guid, startVariables );
			return handle;
		}
		
		Current = Current.Children[guid];
		Current.Clear();
		if ( startVariables is null )
			return handle;
			
		foreach ( var pair in startVariables )
			Current.Add( pair );

		return handle;
	}

	private void Leave()
	{
		if ( Current.Parent is not null )
			Current = Current.Parent!;
	}

	public readonly struct ChildHandle : IDisposable
	{
		private readonly VariableManager<TKey, TValue> _manager;

		public ChildHandle( VariableManager<TKey, TValue> manager )
		{
			_manager = manager;
		}
		
		public void Dispose()
		{
			_manager.Leave();
		}
	}
}
