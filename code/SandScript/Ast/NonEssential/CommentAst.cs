namespace SandScript.AbstractSyntaxTrees;

public class CommentAst : Ast
{
	public readonly string Contents;
	public readonly bool MultiLine;
	
	public CommentAst( TokenLocation location, string contents, bool multiline ) : base(location)
	{
		Contents = contents;
		MultiLine = multiline;
	}
}
