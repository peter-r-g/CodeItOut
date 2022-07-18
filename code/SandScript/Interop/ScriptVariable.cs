using System.Reflection;

namespace SandScript;

/// <summary>
/// Wrapper for a CSharp property. capable of getting/setting at runtime.
/// </summary>
public sealed class ScriptVariable
{
	/// <summary>
	/// The name of the variable.
	/// </summary>
	public readonly string Name;
	/// <summary>
	/// The type of the variable.
	/// </summary>
	public readonly ITypeProvider TypeProvider;

	/// <summary>
	/// Whether or not the variable can be read.
	/// </summary>
	public readonly bool CanRead;
	/// <summary>
	/// Whether or not the variable can be written.
	/// </summary>
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

	/// <summary>
	/// Gets the value of the variable.
	/// </summary>
	/// <returns>The value of the variable.</returns>
	public object? GetValue()
	{
		return _property.GetValue( null );
	}

	/// <summary>
	/// Sets the value of the variable.
	/// </summary>
	/// <param name="value">The value to set on the variable.</param>
	public void SetValue( object? value )
	{
		_property.SetValue( null, value );
	}
}
