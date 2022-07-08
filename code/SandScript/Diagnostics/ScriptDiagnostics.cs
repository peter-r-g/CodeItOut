using System.Collections.Generic;

namespace SandScript;

public class ScriptDiagnostics
{
	public IReadOnlyList<DiagnosticEntry> Errors => _errors;
	private readonly List<DiagnosticEntry> _errors;
	public IReadOnlyList<DiagnosticEntry> Warnings => _warnings;
	private readonly List<DiagnosticEntry> _warnings;
	public IReadOnlyList<DiagnosticEntry> Informationals => _informationals;
	private readonly List<DiagnosticEntry> _informationals;

	internal ScriptDiagnostics()
	{
		_errors = new List<DiagnosticEntry>();
		_warnings = new List<DiagnosticEntry>();
		_informationals = new List<DiagnosticEntry>();
	}

	internal void AddStageAndClear( StageDiagnostics stageDiagnostics )
	{
		_errors.AddRange( stageDiagnostics.Errors );
		_warnings.AddRange( stageDiagnostics.Warnings );
		_informationals.AddRange( stageDiagnostics.Informationals );
		stageDiagnostics.Clear();
	}
}
