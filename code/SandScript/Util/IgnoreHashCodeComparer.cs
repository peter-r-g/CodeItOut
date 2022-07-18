using System.Collections.Generic;

namespace SandScript;

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
