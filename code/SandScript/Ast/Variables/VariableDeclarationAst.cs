using System.Collections.Immutable;

namespace SandScript.AbstractSyntaxTrees;

public sealed class VariableDeclarationAst : Ast
{
	public readonly VariableTypeAst VariableTypeAst;
	public readonly ImmutableArray<VariableAst> VariableNameAsts;
	public readonly Ast DefaultExpressionAst;

	public ITypeProvider VariableType => VariableTypeAst.TypeProvider;
	public string FirstVariableName => VariableNameAsts[0].VariableName;
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
