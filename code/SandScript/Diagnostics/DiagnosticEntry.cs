namespace SandScript;

/// <summary>
/// Contains information about an event that occurred in one of the many stages of execution.
/// </summary>
public class DiagnosticEntry
{
	/// <summary>
	/// The level of the diagnostic.
	/// </summary>
	public readonly DiagnosticLevel Level;
	/// <summary>
	/// The message contained in the diagnostic.
	/// </summary>
	public readonly string Message;
	/// <summary>
	/// The location the diagnostic was raised at.
	/// </summary>
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
