using System.Collections.Immutable;
using SandScript.Exceptions;

namespace SandScript;

public class Script
{
	public ImmutableDictionary<string, ScriptValue> Globals
	{
		get
		{
			var globals = ImmutableDictionary.CreateBuilder<string, ScriptValue>();
			foreach ( var rawGlobal in Interpreter.Variables.Root )
				globals.Add( rawGlobal.Key, ScriptValue.From( rawGlobal.Value ) );
			return globals.ToImmutable();
		}
	}

	protected readonly SemanticAnalyzer Analyzer = new();
	protected readonly Interpreter Interpreter = new();

	public void AddGlobal( string varName, ScriptValue value )
	{
		if ( Analyzer.VariableTypes.Root.ContainsKey( varName ) )
			throw new GlobalRedefinedException( varName );

		var valueTypeProvider = TypeProviders.GetByBackingType( value.Type );
		if ( valueTypeProvider is null )
			throw new TypeUnsupportedException( value.Type );
		
		Analyzer.VariableTypes.Root.Add( varName, valueTypeProvider );;
		Interpreter.Variables.Root.Add( varName, value );

		if ( valueTypeProvider != TypeProviders.Builtin.Method )
			return;

		var method = (ScriptMethod)value.Value!;
		var methodSignature = MethodSignature.From( method );
		Analyzer.VariableMethods.Root.Add( methodSignature, method );
		Interpreter.MethodVariables.Root.Add( methodSignature, value );
	}

	public ScriptValue Call( ScriptMethod method, params object[] args )
	{
		return ScriptValue.From( method.Invoke( Interpreter, args ) );
	}

	public ScriptValue Call( ScriptValue sv, params object[] args )
	{
		if ( sv.TypeProvider != TypeProviders.Builtin.Method )
			throw new TypeMismatchException( TypeProviders.Builtin.Method, sv.TypeProvider );

		return ScriptValue.From( ((ScriptMethod)sv.Value!).Invoke( Interpreter, args ) );
	}

	public ScriptValue Execute( string text ) => Execute( text, out ScriptDiagnostics _ );
	public ScriptValue Execute( string text, out ScriptDiagnostics diagnostics )
	{
		diagnostics = new ScriptDiagnostics();
		
		var tokens = Lexer.Lex( text, false, out var lexerDiagnostics );
		diagnostics.AddStageAndClear( lexerDiagnostics );
		
		var programAst = Parser.Parse( tokens, out var parserDiagnostics );
		diagnostics.AddStageAndClear( parserDiagnostics );
		
		var optimizedProgramAst = Optimizer.Optimize( programAst, out var optimizerDiagnostics );
		diagnostics.AddStageAndClear( optimizerDiagnostics );

		var analysisResult = Analyzer.AnalyzeAst( optimizedProgramAst );
		diagnostics.AddStageAndClear( Analyzer.Diagnostics );
		if ( !analysisResult )
			return null;

		var result = Interpreter.Interpret( optimizedProgramAst );
		diagnostics.AddStageAndClear( Interpreter.Diagnostics );
		
		if ( result is ScriptValue sv )
			return sv;
		
		return ScriptValue.From( result );
	}

	public static Script Execute( string text, out ScriptValue returnValue ) =>
		Execute( text, out returnValue, out _ );
	public static Script Execute( string text, out ScriptValue returnValue, out ScriptDiagnostics diagnostics )
	{
		var script = new Script();
		returnValue = script.Execute( text, out diagnostics );
		return script;
	}
}
