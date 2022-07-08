namespace SandScript.AbstractSyntaxTrees;

public class WhitespaceAst : Ast
{
	public readonly int NumWhitespace;
	
	public WhitespaceAst( TokenLocation startLocation, int numWhitespace ) : base(startLocation)
	{
		NumWhitespace = numWhitespace;
	}
}
