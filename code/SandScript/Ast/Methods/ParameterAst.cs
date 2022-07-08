namespace SandScript.AbstractSyntaxTrees;

public sealed class ParameterAst : Ast
{
	public readonly VariableTypeAst ParameterTypeAst;
	public readonly VariableAst ParameterNameAst;

	public ITypeProvider ParameterType => ParameterTypeAst.TypeProvider;
	public string ParameterName => ParameterNameAst.VariableName;
	
	public ParameterAst( VariableTypeAst parameterTypeAst, VariableAst variableNameAst )
		: base( parameterTypeAst.Token.Location )
	{
		ParameterTypeAst = parameterTypeAst;
		ParameterNameAst = variableNameAst;
	}
}
