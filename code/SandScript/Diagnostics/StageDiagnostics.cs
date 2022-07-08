using System.Collections.Generic;

namespace SandScript;

public class StageDiagnostics
{
	internal readonly List<DiagnosticEntry> Errors = new();
	internal readonly List<DiagnosticEntry> Warnings = new();
	internal readonly List<DiagnosticEntry> Informationals = new();

	protected virtual string StageName => "Stage";

	public void Clear()
	{
		Errors.Clear();
		Warnings.Clear();
		Informationals.Clear();
	}

	public void Error( string message )
	{
		Errors.Add( new DiagnosticEntry( DiagnosticLevel.Error, message, TokenLocation.Zero ) );
	}

	public void Error( string message, TokenLocation location )
	{
		Errors.Add( new DiagnosticEntry( DiagnosticLevel.Error, message, location ) );
	}

	public void Warning( string message )
	{
		Warnings.Add( new DiagnosticEntry( DiagnosticLevel.Warning, message, TokenLocation.Zero ) );
	}

	public void Warning( string message, TokenLocation location )
	{
		Warnings.Add( new DiagnosticEntry( DiagnosticLevel.Warning, message, location ) );
	}

	public void Information( string message )
	{
		Informationals.Add( new DiagnosticEntry( DiagnosticLevel.Informational, message, TokenLocation.Zero ) );
	}

	public void Information( string message, TokenLocation location )
	{
		Informationals.Add( new DiagnosticEntry( DiagnosticLevel.Informational, message, location ) );
	}

	public void Time( double totalMs )
	{
		Information( $"{StageName} took {totalMs}ms" );
	}
}
