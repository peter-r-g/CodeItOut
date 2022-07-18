using System;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public sealed class Interpreter : NodeVisitor<object?>
{
	internal readonly Script Owner;

	internal readonly VariableManager<string, object?> Variables = new(null);
	internal readonly VariableManager<MethodSignature, object?> MethodVariables =
		new(new IgnoreHashCodeComparer<MethodSignature>());
	
	internal readonly InterpreterDiagnostics Diagnostics = new();
	
	internal bool Returning;

	internal Interpreter( Script owner )
	{
		Owner = owner;
		
		foreach ( var method in owner.CustomMethods )
			OwnerOnMethodAdded( owner, method );

		foreach ( var variable in owner.CustomVariables )
			OwnerOnVariableAdded( owner, variable );
		
		owner.MethodAdded += OwnerOnMethodAdded;
		owner.VariableAdded += OwnerOnVariableAdded;
	}

	~Interpreter()
	{
		Owner.MethodAdded -= OwnerOnMethodAdded;
		Owner.VariableAdded -= OwnerOnVariableAdded;
	}

	private void OwnerOnMethodAdded( object sender, ScriptMethod method )
	{
		var methodSignature = MethodSignature.From( method );
		Variables.Root.Add( methodSignature.ToString(), method );
		MethodVariables.Root.Add( methodSignature, method );
	}

	private void OwnerOnVariableAdded( Script sender, ScriptVariable variable )
	{
		Variables.Root.Add( variable.Name, variable );
	}

	public object? Interpret( Ast ast )
	{
		return Visit( ast );
	}

	protected override object? VisitProgram( ProgramAst programAst )
	{
		foreach ( var statement in programAst.Statements )
		{
			var result = Visit( statement );
			if ( !Returning )
				continue;

			Returning = false;
			return result;
		}
		
		return null;
	}

	protected override object? VisitBlock( BlockAst blockAst )
	{
		var guid = blockAst.Guid;
		using var scope = Variables.Enter( guid );
		using var scope2 = MethodVariables.Enter( guid );
		
		foreach ( var statement in blockAst.Statements )
		{
			var result = Visit( statement );
			if ( !Returning )
				continue;

			return result;
		}

		return null;
	}
	
	protected override object? VisitReturn( ReturnAst returnAst )
	{
		Returning = true;
		return Visit( returnAst.ExpressionAst );
	}
	
	protected override object? VisitAssignment( AssignmentAst assignmentAst )
	{
		var variableName = assignmentAst.VariableAst.VariableName;
		Variables.Current.TryGetValue( variableName, out var value, out var container );

		object? newValue;
		if ( assignmentAst.Operator.Type == TokenType.Equals )
		{
			newValue = Visit( assignmentAst.ExpressionAst );
		}
		else
		{
			var binaryOperator = assignmentAst.Operator.Type.GetBinaryOperatorOfAssignment();
			var operation = TypeProviders.GetByValue( value )!.BinaryOperations[binaryOperator];
			newValue = operation( value, Visit( assignmentAst.ExpressionAst ) );
		}

		if ( value is ScriptVariable variable )
			variable.SetValue( newValue );
		else
			container[variableName] = newValue;

		return null;
	}

	protected override object? VisitBinaryOperator( BinaryOperatorAst binaryOperatorAst )
	{
		var left = Visit( binaryOperatorAst.LeftAst );
		var operation = TypeProviders.GetByValue( left )!.BinaryOperations[binaryOperatorAst.Operator.Type];

		return operation( left, Visit( binaryOperatorAst.RightAst ) );
	}

	protected override object? VisitUnaryOperator( UnaryOperatorAst unaryOperatorAst )
	{
		var operand = Visit( unaryOperatorAst.OperandAst );
		var operation = TypeProviders.GetByValue( operand )!.UnaryOperations[unaryOperatorAst.Operator.Type];

		return operation( operand );
	}

	protected override object? VisitIf( IfAst ifAst )
	{
		if ( (bool)Visit( ifAst.BooleanExpressionAst )! )
			return Visit( ifAst.TrueBodyAst );

		return Visit( ifAst.FalseBodyAst );
	}

	protected override object? VisitFor( ForAst forAst )
	{
		using var scope = Variables.Enter( forAst.Guid );
		using var scope2 = MethodVariables.Enter( forAst.Guid );
		Visit( forAst.VariableDeclarationAst );
		while ( (bool)Visit( forAst.BooleanExpressionAst )! )
		{
			var result = Visit( forAst.BodyAst );
			if ( Returning )
				return result;

			Visit( forAst.IteratorAst );
		}
		
		return null;
	}

	protected override object? VisitWhile( WhileAst whileAst )
	{
		while ( (bool)Visit( whileAst.BooleanExpressionAst )! )
		{
			var result = Visit( whileAst.BodyAst );
			if ( Returning )
				return result;
		}

		return null;
	}

	protected override object? VisitDoWhile( DoWhileAst doWhileAst )
	{
		do
		{
			var result = Visit( doWhileAst.BodyAst );
			if ( Returning )
				return result;
		} while ( (bool)Visit( doWhileAst.BooleanExpressionAst )! );

		return null;
	}

	protected override object? VisitMethodDeclaration( MethodDeclarationAst methodDeclarationAst )
	{
		var method = new ScriptMethod( methodDeclarationAst );
		var methodSignature = MethodSignature.From( method );
		
		Variables.Current.AddOrUpdate( methodSignature.ToString(), method );
		MethodVariables.Current.AddOrUpdate( methodSignature, method );

		return null;
	}
	
	protected override object? VisitMethodCall( MethodCallAst methodCallAst )
	{
		MethodVariables.Current.TryGetValue( MethodSignature.From( methodCallAst ), out var variable );

		var arguments = new object?[methodCallAst.ArgumentAsts.Length];
		for ( var i = 0; i < methodCallAst.ArgumentAsts.Length; i++ )
			arguments[i] = Visit( methodCallAst.ArgumentAsts[i] );
		
		return ((ScriptMethod)variable!).Invoke( this, arguments );
	}

	protected override object VisitParameter( ParameterAst parameterAst )
	{
		throw new NotImplementedException();
	}

	protected override object? VisitVariableDeclaration( VariableDeclarationAst variableDeclarationAst )
	{
		var defaultValue = Visit( variableDeclarationAst.DefaultExpressionAst ) ??
		                   variableDeclarationAst.VariableTypeAst.TypeProvider.CreateDefault();

		foreach ( var variable in variableDeclarationAst.VariableNameAsts )
			Variables.Current.AddOrUpdate( variable.VariableName, defaultValue );

		return null;
	}

	protected override object? VisitVariable( VariableAst variableAst )
	{
		Variables.Current.TryGetValue( variableAst.VariableName, out var variable );
		return variable is ScriptVariable sv ? sv.GetValue() : variable;
	}

	protected override object VisitVariableType( VariableTypeAst variableTypeAst )
	{
		throw new NotImplementedException();
	}

	protected override object VisitLiteral( LiteralAst literalAst )
	{
		return literalAst.Value;
	}

	protected override object? VisitNoOperation( NoOperationAst noOperationAst )
	{
		return null;
	}
	
	protected override object VisitComment( CommentAst commentAst )
	{
		throw new NotImplementedException();
	}

	protected override object VisitWhitespace( WhitespaceAst whitespaceAst )
	{
		throw new NotImplementedException();
	}
}
