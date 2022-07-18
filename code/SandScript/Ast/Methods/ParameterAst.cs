namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a parameter of a method.
/// </summary>
public sealed class ParameterAst : Ast
{
	/// <summary>
	/// The variable type of the parameter.
	/// </summary>
	public readonly VariableTypeAst ParameterTypeAst;
	/// <summary>
	/// The variable name of the parameter.
	/// </summary>
	public readonly VariableAst ParameterNameAst;

	/// <summary>
	/// Helper property to quickly retrieve the type from the <see cref="ParameterTypeAst"/>.
	/// </summary>
	public ITypeProvider ParameterType => ParameterTypeAst.TypeProvider;
	/// <summary>
	/// Helper property to quickly retrieve the name of the parameter.
	/// </summary>
	public string ParameterName => ParameterNameAst.VariableName;
	
	public ParameterAst( VariableTypeAst parameterTypeAst, VariableAst variableNameAst )
		: base( parameterTypeAst.Token.Location )
	{
		ParameterTypeAst = parameterTypeAst;
		ParameterNameAst = variableNameAst;
	}
}
