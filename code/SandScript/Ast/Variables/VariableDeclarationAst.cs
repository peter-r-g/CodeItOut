using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a variable declaration statement.
/// </summary>
public sealed class VariableDeclarationAst : Ast
{
	/// <summary>
	/// The type of variable that is being declared.
	/// </summary>
	public readonly VariableTypeAst VariableTypeAst;
	/// <summary>
	/// The list of variable names that are being declared.
	/// </summary>
	public readonly ImmutableArray<VariableAst> VariableNameAsts;
	/// <summary>
	/// The default expression to initialize each variable to.
	/// </summary>
	public readonly Ast DefaultExpressionAst;

	/// <summary>
	/// Helper property to quickly retrieve the type from the <see cref="VariableTypeAst"/>.
	/// </summary>
	public ITypeProvider VariableType => VariableTypeAst.TypeProvider;
	/// <summary>
	/// Helper property to quickly get the first variable name. Useful for statements that require a single declaration like in <see cref="ForAst"/>.
	/// </summary>
	public string FirstVariableName => VariableNameAsts[0].VariableName;
	/// <summary>
	/// Helper property to get the names of all variables being declared.
	/// </summary>
	public ImmutableArray<string> VariableNames
	{
		get
		{
			var variableNames = ImmutableArray.CreateBuilder<string>();
			foreach ( var variableNameAst in VariableNameAsts )
				variableNames.Add( variableNameAst.VariableName );

			return variableNames.ToImmutable();
		}
	}

	public VariableDeclarationAst( VariableTypeAst variableTypeAst, ImmutableArray<VariableAst> variableNameAsts,
		Ast defaultExpressionAst ) : base( variableTypeAst.Token.Location )
	{
		VariableTypeAst = variableTypeAst;
		VariableNameAsts = variableNameAsts;
		DefaultExpressionAst = defaultExpressionAst;
	}
}
