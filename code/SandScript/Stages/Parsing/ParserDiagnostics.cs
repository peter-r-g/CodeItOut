namespace SandScript;

public sealed class ParserDiagnostics : StageDiagnostics
{
	protected override string StageName => "Parsing";

	public void UnexpectedToken( TokenType expectedType, TokenType gotType, TokenLocation location )
	{
		Error( $"Expected {expectedType}, got {gotType}", location );
	}

	public void UnknownVariableType( Token typeToken )
	{
		Error( $"Unknown variable type {typeToken.Value}", typeToken.Location );
	}

	public void UnknownAssignmentOperator( Token assignmentOperatorToken )
	{
		Error( $"Unknown assignment operator {assignmentOperatorToken.Type}", assignmentOperatorToken.Location );
	}

	public void UnhandledLiteralToken( Token literalToken )
	{
		Error( $"The literal \"{literalToken.Value}\" was not consumed", literalToken.Location );
	}

	public void UnexpectedResult( string expectedResult, Token gotToken )
	{
		Error( $"Expected {expectedResult}, got {gotToken.Type}", gotToken.Location );
	}
}
