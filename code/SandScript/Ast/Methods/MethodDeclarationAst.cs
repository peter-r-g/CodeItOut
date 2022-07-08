using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

public sealed class MethodDeclarationAst : Ast
{
	public readonly VariableTypeAst ReturnTypeAst;
	public readonly VariableAst MethodNameAst;
	public readonly ImmutableArray<ParameterAst> ParameterAsts;
	public readonly BlockAst BodyAst;

	public ITypeProvider ReturnType => ReturnTypeAst.TypeProvider;
	public string MethodName => MethodNameAst.VariableName;

	public MethodDeclarationAst( VariableTypeAst returnTypeAst, VariableAst methodNameAst,
		ImmutableArray<ParameterAst> parameterAsts, BlockAst bodyAst ) : base( returnTypeAst.Token.Location )
	{
		ReturnTypeAst = returnTypeAst;
		MethodNameAst = methodNameAst;
		ParameterAsts = parameterAsts;
		BodyAst = bodyAst;
	}
}
