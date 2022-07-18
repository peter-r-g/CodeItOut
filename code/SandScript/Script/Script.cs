using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
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

	internal delegate void MethodAddedEventHandler( Script sender, ScriptMethod method );
	internal event MethodAddedEventHandler MethodAdded;
	internal IEnumerable<ScriptMethod> CustomMethods => _customMethodCache;
	private readonly List<ScriptMethod> _customMethodCache = new();

	internal delegate void VariableAddedEventHandler( Script sender, ScriptVariable variable );
	internal event VariableAddedEventHandler VariableAdded;
	internal IEnumerable<ScriptVariable> CustomVariables => _customVariableCache;
	private readonly List<ScriptVariable> _customVariableCache = new();

	protected readonly SemanticAnalyzer Analyzer;
	protected readonly Interpreter Interpreter;

	public Script()
	{
		Analyzer = new SemanticAnalyzer( this );
		Interpreter = new Interpreter( this );
	}

	public void AddClassMethods<T>() where T : class
	{
		var methods = TypeLibrary.FindStaticMethods<ScriptMethodAttribute>();

		foreach ( var method in methods )
		{
			if ( method.TypeDescription.TargetType != typeof(T) )
				continue;

			var returnTypeProvider = SandboxHelper.GetReturnTypeProvider( method, out var returnType );
			if ( returnTypeProvider is null )
				throw new ReturnTypeUnsupportedException( returnType );
			
			var parameters = SandboxHelper.GetParameters( method );
			if ( parameters.Count == 0 || parameters[0].ParameterType != typeof(Script) )
				throw new ParameterException( "First parameter must be of type " + nameof(Script) + "." );

			for ( var i = 1; i < parameters.Count; i++ )
			{
				var parameter = parameters[i];
				if ( parameter.ParameterType != typeof(ScriptValue) && parameter.ParameterTypeProvider is null )
					throw new ParameterException( "Parameter type \"" + parameter.ParameterType + "\" is unsupported." );
			}

			var methodNameAttributes = SandboxHelper.GetNames( method );
			foreach ( var methodNameAttribute in methodNameAttributes )
			{
				var scriptMethod = new ScriptMethod( method, methodNameAttribute );
				_customMethodCache.Add( scriptMethod );
				MethodAdded?.Invoke( this, scriptMethod );
			}
		}
	}

	public void AddClassVariables<T>() where T : class
	{
		var properties = typeof(T).GetProperties( BindingFlags.Public | BindingFlags.Static )
			.Where( p => p.GetCustomAttributes( typeof(ScriptVariableAttribute), false ).Length > 0 );

		foreach ( var property in properties )
		{
			if ( TypeProviders.GetByBackingType( property.PropertyType ) is null )
				throw new TypeUnsupportedException( property.PropertyType );

			foreach ( var attribute in property.GetCustomAttributes<ScriptVariableAttribute>() )
			{
				if ( attribute.CanRead && !property.CanRead )
					throw new UnreadableVariableException( property, attribute );

				if ( attribute.CanWrite && !property.CanWrite )
					throw new UnwritableVariableException( property, attribute );

				var variable = new ScriptVariable( property, attribute );
				_customVariableCache.Add( variable );
				VariableAdded?.Invoke( this, variable );
			}
		}
	}

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
