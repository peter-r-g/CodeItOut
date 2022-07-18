using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using SandScript.Exceptions;

namespace SandScript;

/// <summary>
/// The core interaction of SandScript.
/// Contains and executes pieces of SandScript code and retains state in the <see cref="SemanticAnalyzer"/> and <see cref="Interpreter"/>.
/// </summary>
public class Script
{
	/// <summary>
	/// Property to grab all globals that are in this script context.
	/// </summary>
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
	internal event MethodAddedEventHandler? MethodAdded;
	internal IEnumerable<ScriptMethod> CustomMethods => _customMethodCache;
	private readonly List<ScriptMethod> _customMethodCache = new();

	internal delegate void VariableAddedEventHandler( Script sender, ScriptVariable variable );
	internal event VariableAddedEventHandler? VariableAdded;
	internal IEnumerable<ScriptVariable> CustomVariables => _customVariableCache;
	private readonly List<ScriptVariable> _customVariableCache = new();

	protected readonly SemanticAnalyzer Analyzer;
	protected readonly Interpreter Interpreter;

	public Script()
	{
		Analyzer = new SemanticAnalyzer( this );
		Interpreter = new Interpreter( this );
	}

	/// <summary>
	/// Adds all methods marked with the <see cref="ScriptMethodAttribute"/> to this scripts <see cref="SemanticAnalyzer"/> and <see cref="Interpreter"/>.
	/// </summary>
	/// <typeparam name="T">The class to search for methods.</typeparam>
	/// <exception cref="ReturnTypeUnsupportedException">Thrown when a method has a return type that is not supported.</exception>
	/// <exception cref="ParameterException">Thrown when a parameter on a method has a type that is not supported.</exception>
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

	/// <summary>
	/// Adds all properties marked with the <see cref="ScriptVariableAttribute"/> to this scripts <see cref="SemanticAnalyzer"/> and <see cref="Interpreter"/>.
	/// </summary>
	/// <typeparam name="T">The class to search for properties.</typeparam>
	/// <exception cref="TypeUnsupportedException">Thrown when the property is of a type that is not supported.</exception>
	/// <exception cref="UnreadableVariableException">Thrown when the property has no getter when it should.</exception>
	/// <exception cref="UnwritableVariableException">Thrown when the property has no setter when it should.</exception>
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

	/// <summary>
	/// Adds a global variable to the script.
	/// </summary>
	/// <param name="varName">The name the variable should have.</param>
	/// <param name="value">The value the variable should be set to.</param>
	/// <exception cref="GlobalRedefinedException">Thrown when a variable of the same name already exists.</exception>
	/// <exception cref="TypeUnsupportedException">Thrown when the <see cref="ScriptValue"/> type is not supported.</exception>
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

	/// <summary>
	/// Calls a method in this scripts context with the arguments passed.
	/// </summary>
	/// <param name="method">The method to call.</param>
	/// <param name="args">The array of arguments to pass to the method.</param>
	/// <returns>The returned value of the method wrapped in a <see cref="ScriptValue"/></returns>
	public ScriptValue Call( ScriptMethod method, params object?[] args )
	{
		return ScriptValue.From( method.Invoke( Interpreter, args ) );
	}

	/// <summary>
	/// Calls a <see cref="ScriptValue"/> like a method. See <see cref="Call(SandScript.ScriptMethod,object?[])"/>
	/// </summary>
	/// <param name="sv">The <see cref="ScriptValue"/> to call.</param>
	/// <param name="args">The array of arguments to pass to the method.</param>
	/// <returns>The returned value of the method wrapped in a <see cref="ScriptValue"/></returns>
	/// <exception cref="TypeMismatchException">Thrown when the  </exception>
	public ScriptValue Call( ScriptValue sv, params object?[] args )
	{
		if ( sv.TypeProvider != TypeProviders.Builtin.Method )
			throw new TypeMismatchException( TypeProviders.Builtin.Method, sv.TypeProvider );

		return ScriptValue.From( ((ScriptMethod)sv.Value!).Invoke( Interpreter, args ) );
	}

	/// <summary>
	/// Executes a string of code.
	/// </summary>
	/// <param name="text">The code to execute.</param>
	/// <returns>The returned value of the code.</returns>
	public ScriptValue? Execute( string text ) => Execute( text, out ScriptDiagnostics _ );
	/// <summary>
	/// Executes a string of code.
	/// </summary>
	/// <param name="text">The code to execute.</param>
	/// <param name="diagnostics">The diagnostics raised.</param>
	/// <returns>The returned value of the code.</returns>
	public ScriptValue? Execute( string text, out ScriptDiagnostics diagnostics )
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

	/// <summary>
	/// Executes a string of code and returns the resulting <see cref="Script"/>.
	/// </summary>
	/// <param name="text">The code to execute.</param>
	/// <param name="returnValue">The returned value of the code.</param>
	/// <returns>The created script.</returns>
	public static Script Execute( string text, out ScriptValue? returnValue ) =>
		Execute( text, out returnValue, out _ );
	/// <summary>
	/// Executes a string of code and returns the resuling <see cref="Script"/>.
	/// </summary>
	/// <param name="text">The code to execute.</param>
	/// <param name="returnValue">The returned value of the code.</param>
	/// <param name="diagnostics">The diagnostics raised.</param>
	/// <returns>The created script.</returns>
	public static Script Execute( string text, out ScriptValue? returnValue, out ScriptDiagnostics diagnostics )
	{
		var script = new Script();
		returnValue = script.Execute( text, out diagnostics );
		return script;
	}
}
