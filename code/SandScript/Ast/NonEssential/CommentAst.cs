namespace SandScript.AbstractSyntaxTrees;

/// <summary>
/// Represents a comment.
/// </summary>
public class CommentAst : Ast
{
	/// <summary>
	/// The string contents of the comment.
	/// </summary>
	public readonly string Contents;
	/// <summary>
	/// Whether or not this is a multiline comment.
	/// </summary>
	public readonly bool MultiLine;
	
	public CommentAst( TokenLocation location, string contents, bool multiline ) : base(location)
	{
		Contents = contents;
		MultiLine = multiline;
	}
}
