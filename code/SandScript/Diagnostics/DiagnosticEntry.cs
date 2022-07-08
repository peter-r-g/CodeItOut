namespace SandScript;

public class DiagnosticEntry
{
	public readonly DiagnosticLevel Level;
	public readonly string Message;
	public readonly TokenLocation Location;

	public DiagnosticEntry( DiagnosticLevel level, string message, TokenLocation location )
	{
		Level = level;
		Message = message;
		Location = location;
	}

	public override string ToString()
	{
		if ( Location == TokenLocation.Zero )
			return Message;

		return Message + " at " + Location;
	}
}
