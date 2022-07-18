using System.Collections.Generic;

namespace SandScript;

/// <summary>
/// Container for holding all diagnostics raised in a single execution stage.
/// </summary>
public class StageDiagnostics
{
	/// <summary>
	/// Container for all errors raised.
	/// </summary>
	internal readonly List<DiagnosticEntry> Errors = new();
	/// <summary>
	/// Container for all warnings raised.
	/// </summary>
	internal readonly List<DiagnosticEntry> Warnings = new();
	/// <summary>
	/// Container for all information raised.
	/// </summary>
	internal readonly List<DiagnosticEntry> Informationals = new();

	/// <summary>
	/// Property to name the diagnostics stage.
	/// </summary>
	protected virtual string StageName => "Stage";

	/// <summary>
	/// Clears all diagnostics.
	/// </summary>
	public void Clear()
	{
		Errors.Clear();
		Warnings.Clear();
		Informationals.Clear();
	}

	/// <summary>
	/// Adds a new error diagnostic.
	/// </summary>
	/// <param name="message">The message attached to the error.</param>
	public void Error( string message )
	{
		Errors.Add( new DiagnosticEntry( DiagnosticLevel.Error, message, TokenLocation.Zero ) );
	}

	/// <summary>
	/// Adds a new error diagnostic.
	/// </summary>
	/// <param name="message">The message attached to the error.</param>
	/// <param name="location">The location of the error.</param>
	public void Error( string message, TokenLocation location )
	{
		Errors.Add( new DiagnosticEntry( DiagnosticLevel.Error, message, location ) );
	}

	/// <summary>
	/// Adds a new warning diagnostic.
	/// </summary>
	/// <param name="message">The message attached to the warning.</param>
	public void Warning( string message )
	{
		Warnings.Add( new DiagnosticEntry( DiagnosticLevel.Warning, message, TokenLocation.Zero ) );
	}

	/// <summary>
	/// Adds a new warning diagnostic.
	/// </summary>
	/// <param name="message">The message attached to the warning.</param>
	/// <param name="location">The location of the warning.</param>
	public void Warning( string message, TokenLocation location )
	{
		Warnings.Add( new DiagnosticEntry( DiagnosticLevel.Warning, message, location ) );
	}

	/// <summary>
	/// Adds a new informational diagnostic.
	/// </summary>
	/// <param name="message">The message attached to the information.</param>
	public void Information( string message )
	{
		Informationals.Add( new DiagnosticEntry( DiagnosticLevel.Informational, message, TokenLocation.Zero ) );
	}

	/// <summary>
	/// Adds a new informational diagnostic.
	/// </summary>
	/// <param name="message">The message attached to the information.</param>
	/// <param name="location">The location of the information.</param>
	public void Information( string message, TokenLocation location )
	{
		Informationals.Add( new DiagnosticEntry( DiagnosticLevel.Informational, message, location ) );
	}

	/// <summary>
	/// Adds an informational diagnostic showing how long the stage took to execute.
	/// </summary>
	/// <param name="totalMs">The time in milliseconds the stage took to execute.</param>
	public void Time( double totalMs )
	{
		Information( $"{StageName} took {totalMs}ms" );
	}
}
