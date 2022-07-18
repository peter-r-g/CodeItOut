using System.Diagnostics.CodeAnalysis;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public sealed class SemanticAnalyzer : NodeVisitor<ITypeProvider>
{
	private readonly Script? _owner;
	
	internal readonly VariableManager<string, ITypeProvider> VariableTypes = new(null);
	internal readonly VariableManager<MethodSignature, ScriptMethod> VariableMethods =
		new(new IgnoreHashCodeComparer<MethodSignature>());
	private readonly VariableManager<string, ScriptVariable> _variableExternals = new(null);
	
	internal readonly SemanticAnalyzerDiagnostics Diagnostics = new();
	
	private readonly TypeCheckStack _neededTypes = new();

	internal SemanticAnalyzer()
	{
		_owner = null;
	}

	internal SemanticAnalyzer( Script owner )
	{
		_owner = owner;
		
		foreach ( var method in owner.CustomMethods )
			OwnerOnMethodAdded( owner, method );

		foreach ( var variable in owner.CustomVariables )
			OwnerOnVariableAdded( owner, variable );
		
		owner.MethodAdded += OwnerOnMethodAdded;
		owner.VariableAdded += OwnerOnVariableAdded;
	}

	~SemanticAnalyzer()
	{
		if ( _owner is null )
			return;
		
		_owner.MethodAdded -= OwnerOnMethodAdded;
		_owner.VariableAdded -= OwnerOnVariableAdded;
	}

	private void OwnerOnMethodAdded( Script sender, ScriptMethod method )
	{
		var methodSignature = MethodSignature.From( method );
		VariableTypes.Root.AddOrUpdate( methodSignature.ToString(), TypeProviders.Builtin.Method );
		VariableMethods.Root.AddOrUpdate( methodSignature, method );
	}
	
	private void OwnerOnVariableAdded( Script sender, ScriptVariable variable )
	{
		VariableTypes.Root.AddOrUpdate( variable.Name, variable.TypeProvider );
		_variableExternals.Root.AddOrUpdate( variable.Name, variable );
	}

	public bool AnalyzeAst( Ast ast )
	{
		Visit( ast );
		
		return Diagnostics.Errors.Count == 0;
	}

	private ITypeProvider VisitExpectingType( ITypeProvider type, Ast ast )
	{
		_neededTypes.Push( type );
		var result = Visit( ast );
		_neededTypes.Pop();

		return result;
	}

	private bool VerifyTypeLoose( ITypeProvider type, [NotNullWhen( false )] out ITypeProvider? expectedType )
	{
		return _neededTypes.AssertTypeCheckLoose( type, out expectedType );
	}

	protected override ITypeProvider VisitProgram( ProgramAst programAst )
	{
		foreach ( var statement in programAst.Statements )
			Visit( statement );

		return TypeProviders.Builtin.Nothing;
	}

	protected override ITypeProvider VisitBlock( BlockAst blockAst )
	{
		var guid = blockAst.Guid;
		using var scope = VariableTypes.Enter( guid );
		using var scope2 = VariableMethods.Enter( guid );
		using var scope3 = _variableExternals.Enter( guid );
		foreach ( var statement in blockAst.Statements )
			Visit( statement );

		return TypeProviders.Builtin.Nothing;
	}
	
	protected override ITypeProvider VisitReturn( ReturnAst returnAst )
	{
		var result = Visit( returnAst.ExpressionAst );
		if ( !VerifyTypeLoose( result, out var expectedType ) )
			Diagnostics.TypeMismatch( expectedType, result, returnAst.StartLocation );
		
		return result;
	}
	
	protected override ITypeProvider VisitAssignment( AssignmentAst assignmentAst )
	{
		var variableName = assignmentAst.VariableName;
		if ( !VariableTypes.Current.TryGetValue( variableName, out var type ) )
		{
			Diagnostics.Undefined( variableName, assignmentAst.StartLocation );
			return TypeProviders.Builtin.Nothing;
		}

		if ( _variableExternals.Current.TryGetValue( variableName, out var variable ) && !variable.CanWrite )
		{
			Diagnostics.Unwritable( variableName );
			return TypeProviders.Builtin.Nothing;
		}

		_neededTypes.Push( type );
		if ( assignmentAst.OperatorType.GetBinaryOperatorOfAssignment() != TokenType.None )
			Visit( assignmentAst.ExpressionAst );
		_neededTypes.Pop();

		return TypeProviders.Builtin.Nothing;
	}
	
	protected override ITypeProvider VisitBinaryOperator( BinaryOperatorAst binaryOperatorAst )
	{
		var leftType = VisitExpectingType( TypeProviders.Builtin.Variable, binaryOperatorAst.LeftAst );
		VisitExpectingType( leftType, binaryOperatorAst.RightAst );

		var operatorType = binaryOperatorAst.OperatorType;
		var operandResultType = operatorType.GetOperatorResultType( leftType );
		
		if ( !VerifyTypeLoose( operandResultType, out var expectedType ) )
			Diagnostics.TypeMismatch( expectedType, leftType, binaryOperatorAst.StartLocation );

		if ( leftType.BinaryOperations.ContainsKey( operatorType ) )
			return leftType;

		Diagnostics.UnsupportedBinaryOperatorForType( operatorType, leftType, binaryOperatorAst.StartLocation );
		return leftType;
	}

	protected override ITypeProvider VisitUnaryOperator( UnaryOperatorAst unaryOperatorAst )
	{
		var operandType = VisitExpectingType( TypeProviders.Builtin.Variable, unaryOperatorAst.OperandAst );

		var operatorType = unaryOperatorAst.OperatorType;
		var operandResultType = operatorType.GetOperatorResultType( operandType );
		
		if ( !VerifyTypeLoose( operandResultType, out var expectedType ) )
			Diagnostics.TypeMismatch( expectedType, operandType, unaryOperatorAst.StartLocation );

		if ( operandType.UnaryOperations.ContainsKey( operatorType ) )
			return operandType;
		
		Diagnostics.UnsupportedUnaryOperatorForType( operatorType, operandType, unaryOperatorAst.StartLocation );
		return operandType;
	}
	
	protected override ITypeProvider VisitIf( IfAst ifAst )
	{
		VisitExpectingType( TypeProviders.Builtin.Boolean, ifAst.BooleanExpressionAst );
		
		var result = Visit( ifAst.TrueBodyAst );
		if ( !VerifyTypeLoose( result, out var expectedType ) )
			Diagnostics.TypeMismatch( expectedType, result, ifAst.TrueBodyAst.StartLocation );

		if ( ifAst.FalseBodyAst is NoOperationAst )
			return result;
		
		result = Visit( ifAst.FalseBodyAst );
		if ( !VerifyTypeLoose( result, out expectedType ) )
			Diagnostics.TypeMismatch( expectedType, result, ifAst.FalseBodyAst.StartLocation );

		return result;
	}

	protected override ITypeProvider VisitFor( ForAst forAst )
	{
		var guid = forAst.Guid;
		using var scope = VariableTypes.Enter( guid );
		using var scope2 = VariableMethods.Enter( guid );
		using var scope3 = _variableExternals.Enter( guid );
		VisitExpectingType( TypeProviders.Builtin.Number, forAst.VariableDeclarationAst );
		VisitExpectingType( TypeProviders.Builtin.Boolean, forAst.BooleanExpressionAst );
		VisitExpectingType( TypeProviders.Builtin.Nothing, forAst.IteratorAst );
		var result = Visit( forAst.BodyAst );

		return result;
	}
	
	protected override ITypeProvider VisitWhile( WhileAst whileAst )
	{
		VisitExpectingType( TypeProviders.Builtin.Boolean, whileAst.BooleanExpressionAst );
		return Visit( whileAst.BodyAst );
	}

	protected override ITypeProvider VisitDoWhile( DoWhileAst doWhileAst )
	{
		VisitExpectingType( TypeProviders.Builtin.Boolean, doWhileAst.BooleanExpressionAst );
		return Visit( doWhileAst.BodyAst );
	}

	protected override ITypeProvider VisitMethodDeclaration( MethodDeclarationAst methodDeclarationAst )
	{
		var method = new ScriptMethod( methodDeclarationAst );
		var methodSignature = MethodSignature.From( method );
		if ( VariableMethods.Current.TryGetValue( methodSignature, out _, out var container ) )
		{
			Diagnostics.Redefined( methodSignature.ToString(), container.Guid );
			return TypeProviders.Builtin.Nothing;
		}
		
		VariableTypes.Current.AddOrUpdate( methodSignature.ToString(), TypeProviders.Builtin.Method );
		VariableMethods.Current.AddOrUpdate( methodSignature, new ScriptMethod( methodDeclarationAst ) );

		var guid = methodDeclarationAst.Guid;
		using var scope = VariableTypes.Enter( guid );
		using var scope2 = VariableMethods.Enter( guid );
		using var scope3 = _variableExternals.Enter( guid );
		foreach ( var parameter in methodDeclarationAst.ParameterAsts )
			Visit( parameter );
		Visit( methodDeclarationAst.BodyAst );
			
		return TypeProviders.Builtin.Nothing;
	}

	protected override ITypeProvider VisitMethodCall( MethodCallAst methodCallAst )
	{
		foreach ( var argument in methodCallAst.ArgumentAsts )
		{
			var argumentType = VisitExpectingType( TypeProviders.Builtin.Variable, argument );
			methodCallAst.ArgumentTypes = methodCallAst.ArgumentTypes.Add( argumentType );
		}

		var callSignature = MethodSignature.From( methodCallAst );
		if ( !VariableMethods.Current.TryGetValue( callSignature, out var method ) )
		{
			Diagnostics.Undefined( callSignature.ToString(), methodCallAst.StartLocation );
			return TypeProviders.Builtin.Nothing;
		}
		
		if ( !VerifyTypeLoose( method.ReturnTypeProvider, out var expectedType ) )
			Diagnostics.TypeMismatch( expectedType, method.ReturnTypeProvider, methodCallAst.StartLocation );

		var numArguments = methodCallAst.ArgumentAsts.Length;
		if ( numArguments != method.Parameters.Count )
			Diagnostics.ArgumentCountMismatch( method.Parameters.Count, numArguments, methodCallAst.StartLocation );

		for ( var i = 0; i < numArguments; i++ )
		{
			var parameter = method.Parameters[i];
			if ( i >= method.Parameters.Count )
			{
				Diagnostics.MissingParameter( parameter.Item1, methodCallAst.MethodName, methodCallAst.StartLocation );
				continue;
			}
			
			VisitExpectingType( parameter.Item2, methodCallAst.ArgumentAsts[i] );
		}
		
		return method.ReturnTypeProvider;
	}

	protected override ITypeProvider VisitParameter( ParameterAst parameterAst )
	{
		VariableTypes.Current.AddOrUpdate( parameterAst.ParameterName, parameterAst.ParameterType );
		return parameterAst.ParameterType;
	}

	protected override ITypeProvider VisitVariableDeclaration( VariableDeclarationAst variableDeclarationAst )
	{
		var value = VisitExpectingType( variableDeclarationAst.VariableType,
			variableDeclarationAst.DefaultExpressionAst );
		if ( value == TypeProviders.Builtin.Nothing )
			value = variableDeclarationAst.VariableType;
		
		if ( value == TypeProviders.Builtin.Nothing || value == TypeProviders.Builtin.Variable )
			Diagnostics.MissingType( variableDeclarationAst.StartLocation );
		
		foreach ( var variable in variableDeclarationAst.VariableNameAsts )
		{
			var variableName = variable.VariableName;
			
			if ( VariableTypes.Current.TryGetValue( variableName, out _, out var container ) )
			{
				Diagnostics.Redefined( variableName, container.Guid );
				continue;
			}

			VariableTypes.Current.AddOrUpdate( variableName, value );
		}

		return TypeProviders.Builtin.Nothing;
	}

	protected override ITypeProvider VisitVariable( VariableAst variableAst )
	{
		var variableName = variableAst.VariableName;
		if ( !VariableTypes.Current.TryGetValue( variableName, out var variableType ) )
		{
			Diagnostics.Undefined( variableName, variableAst.StartLocation );
			return TypeProviders.Builtin.Nothing;
		}

		if ( _variableExternals.Current.TryGetValue( variableName, out var variable ) && !variable.CanRead )
		{
			Diagnostics.Unreadable( variableName );
			return variable.TypeProvider;
		}
		
		if ( !VerifyTypeLoose( variableType, out var expectedType ) )
			Diagnostics.TypeMismatch( expectedType, variableType, variableAst.StartLocation );
		
		return variableType;
	}

	protected override ITypeProvider VisitVariableType( VariableTypeAst variableTypeAst )
	{
		return variableTypeAst.TypeProvider;
	}

	protected override ITypeProvider VisitLiteral( LiteralAst literalAst )
	{
		var typeProvider = literalAst.TypeProvider;
		if ( !VerifyTypeLoose( typeProvider, out var expectedType ) )
			Diagnostics.TypeMismatch( expectedType, typeProvider, literalAst.StartLocation );
		
		return typeProvider;
	}

	protected override ITypeProvider VisitNoOperation( NoOperationAst noOperationAst )
	{
		return TypeProviders.Builtin.Nothing;
	}

	protected override ITypeProvider VisitComment( CommentAst commentAst )
	{
		return TypeProviders.Builtin.Nothing;
	}

	protected override ITypeProvider VisitWhitespace( WhitespaceAst whitespaceAst )
	{
		return TypeProviders.Builtin.Nothing;
	}

	public static bool Analyze( Ast ast ) => Analyze( ast, out _ );
	public static bool Analyze( Ast ast, out StageDiagnostics diagnostics )
	{
		var analyzer = new SemanticAnalyzer();
		var result = analyzer.AnalyzeAst( ast );
		diagnostics = analyzer.Diagnostics;

		return result;
	}
}
