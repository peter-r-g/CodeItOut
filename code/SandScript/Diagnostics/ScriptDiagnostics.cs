using System.Collections.Generic;

namespace SandScript;

/// <summary>
/// Container for holding all diagnostics across many stages.
/// </summary>
public class ScriptDiagnostics
{
	/// <summary>
	/// List of all errors raised.
	/// </summary>
	public IReadOnlyList<DiagnosticEntry> Errors => _errors;
	private readonly List<DiagnosticEntry> _errors;
	/// <summary>
	/// List of all warnings raised.
	/// </summary>
	public IReadOnlyList<DiagnosticEntry> Warnings => _warnings;
	private readonly List<DiagnosticEntry> _warnings;
	/// <summary>
	/// List of all information raised.
	/// </summary>
	public IReadOnlyList<DiagnosticEntry> Informationals => _informationals;
	private readonly List<DiagnosticEntry> _informationals;

	internal ScriptDiagnostics()
	{
		_errors = new List<DiagnosticEntry>();
		_warnings = new List<DiagnosticEntry>();
		_informationals = new List<DiagnosticEntry>();
	}

	/// <summary>
	/// Adds a stages diagnostics then clears it for future use.
	/// </summary>
	/// <param name="stageDiagnostics">The stage to add and clear.</param>
	internal void AddStageAndClear( StageDiagnostics stageDiagnostics )
	{
		_errors.AddRange( stageDiagnostics.Errors );
		_warnings.AddRange( stageDiagnostics.Warnings );
		_informationals.AddRange( stageDiagnostics.Informationals );
		stageDiagnostics.Clear();
	}
}
