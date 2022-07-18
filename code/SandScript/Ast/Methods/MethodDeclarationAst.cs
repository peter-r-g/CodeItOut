using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a method declaration statement.
/// </summary>
public sealed class MethodDeclarationAst : Ast
{
	/// <summary>
	/// The variable return type of the declaration.
	/// </summary>
	public readonly VariableTypeAst ReturnTypeAst;
	/// <summary>
	/// The variable name of the method declaration.
	/// </summary>
	public readonly VariableAst MethodNameAst;
	/// <summary>
	/// The list of parameters this method takes.
	/// </summary>
	public readonly ImmutableArray<ParameterAst> ParameterAsts;
	/// <summary>
	/// The body of the method to execute.
	/// </summary>
	public readonly BlockAst BodyAst;

	/// <summary>
	/// Helper property to quickly retrieve the return type from the <see cref="ReturnTypeAst"/>.
	/// </summary>
	public ITypeProvider ReturnType => ReturnTypeAst.TypeProvider;
	/// <summary>
	/// Helper property to quickly retrieve the name of the method being declared.
	/// </summary>
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
