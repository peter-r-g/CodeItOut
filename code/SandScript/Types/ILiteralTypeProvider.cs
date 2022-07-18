using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public interface ILiteralTypeProvider : ITypeProvider
{
	object? GetLiteral( Lexer lexer );
	LiteralAst? GetLiteralAst( Token token );
}
