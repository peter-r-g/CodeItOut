using System.Reflection;

namespace SandScript;

public sealed class ScriptVariable
{
	public readonly string Name;
	public readonly ITypeProvider TypeProvider;

	public readonly bool CanRead;
	public readonly bool CanWrite;

	private readonly PropertyInfo _property;

	internal ScriptVariable( PropertyInfo propertyInfo, ScriptVariableAttribute attribute )
	{
		Name = attribute.VariableName;
		TypeProvider = TypeProviders.GetByBackingType( propertyInfo.PropertyType )!;

		CanRead = attribute.CanRead;
		CanWrite = attribute.CanWrite;
		
		_property = propertyInfo;
	}

	public object? GetValue()
	{
		return _property.GetValue( null );
	}

	public void SetValue( object? value )
	{
		_property.SetValue( null, value );
	}
}
