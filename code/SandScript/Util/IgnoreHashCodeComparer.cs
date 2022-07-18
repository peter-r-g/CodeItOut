using System.Collections.Generic;

namespace SandScript;

/// <summary>
/// Comparer that ignores has code comparing so that values can be tested for equality.
/// </summary>
/// <typeparam name="T">The types of values to be compared.</typeparam>
public class IgnoreHashCodeComparer<T> : IEqualityComparer<T>
{
	public bool Equals( T? x, T? y )
	{
		if ( x is null )
			return y is null;

		return x.Equals( y );
	}

	public int GetHashCode( T obj )
	{
		return 0;
	}
}
