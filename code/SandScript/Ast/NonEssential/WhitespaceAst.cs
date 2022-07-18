namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents whitespace.
/// </summary>
public class WhitespaceAst : Ast
{
	/// <summary>
	/// The amount of whitespace consumed.
	/// </summary>
	// TODO: This should just be a string of the whitespace as it doesn't save new lines.
	public readonly int NumWhitespace;
	
	public WhitespaceAst( TokenLocation startLocation, int numWhitespace ) : base(startLocation)
	{
		NumWhitespace = numWhitespace;
	}
}
