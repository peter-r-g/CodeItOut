namespace SandScript;

public sealed class LexerDiagnostics : StageDiagnostics
{
	protected override string StageName => "Lexical analysis";

	public void NoCode()
	{
		Error( "No code provided" );
	}

	public void UnknownToken( char token, TokenLocation location )
	{
		Error( $"Unknown token '{token}'", location );
	}

	public void UnclosedCharacter( TokenLocation location )
	{
		Error( $"Character starting at {location} was not closed", location );
	}

	public void UnclosedString( TokenLocation location )
	{
		Error( $"String starting at {location} was not closed", location );
	}
}
