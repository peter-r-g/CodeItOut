using System.Collections.Immutable;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public sealed class Optimizer : NodeVisitor<Ast>
{
	private readonly VariableManager<MethodSignature, bool> _removedMethods =
		new(new IgnoreHashCodeComparer<MethodSignature>());

	private int _numChanges;
	
	private readonly OptimizerDiagnostics _diagnostics = new();

	private Optimizer()
	{
	}

	private Ast OptimizeTree( Ast ast )
	{
		return Visit( ast );
	}

	private Ast AddChange( Ast ast )
	{
		_numChanges++;
		return ast;
	}

	protected override Ast VisitProgram( ProgramAst programAst )
	{
		var newStatements = ImmutableArray.CreateBuilder<Ast>();
		foreach ( var statement in programAst.Statements )
		{
			var result = Visit( statement );
			if ( result is not NoOperationAst )
				newStatements.Add( result );
		}
		
		return newStatements.Count == 0
			? AddChange( new NoOperationAst( programAst.StartLocation ) )
			: new ProgramAst( newStatements.ToImmutable() );
	}

	protected override Ast VisitBlock( BlockAst blockAst )
	{
		var newStatements = ImmutableArray.CreateBuilder<Ast>();
		foreach ( var statement in blockAst.Statements )
		{
			var result = Visit( statement );
			if ( result is not NoOperationAst )
				newStatements.Add( result );
		}

		return newStatements.Count == 0
			? AddChange( new NoOperationAst( blockAst.StartLocation ) )
			: new BlockAst( blockAst.StartLocation, newStatements.ToImmutable() );
	}

	protected override Ast VisitReturn( ReturnAst returnAst )
	{
		return new ReturnAst( returnAst.StartLocation, Visit( returnAst.ExpressionAst ) );
	}

	protected override Ast VisitAssignment( AssignmentAst assignmentAst )
	{
		var newExpression = Visit( assignmentAst.ExpressionAst );
		return new AssignmentAst( assignmentAst.VariableAst, assignmentAst.Operator, newExpression );
	}

	protected override Ast VisitBinaryOperator( BinaryOperatorAst binaryOperatorAst )
	{
		var newLeft = Visit( binaryOperatorAst.LeftAst );
		var newRight = Visit( binaryOperatorAst.RightAst );
		
		if ( newLeft is not LiteralAst leftLiteral || newRight is not LiteralAst rightLiteral )
			return new BinaryOperatorAst( newLeft, binaryOperatorAst.Operator, newRight );
		
		var binaryOperation = leftLiteral.TypeProvider.BinaryOperations[binaryOperatorAst.Operator.Type];
		var newValue = binaryOperation( leftLiteral.Value, rightLiteral.Value );
		var newToken = new Token( TokenType.Literal, newValue!, leftLiteral.StartLocation );
		return AddChange( new LiteralAst( newToken, leftLiteral.TypeProvider ) );
	}

	protected override Ast VisitUnaryOperator( UnaryOperatorAst unaryOperatorAst )
	{
		var newOperand = Visit( unaryOperatorAst.OperandAst );
		if ( newOperand is not LiteralAst literalAst )
			return new UnaryOperatorAst( unaryOperatorAst.Operator, newOperand );

		var unaryOperation = literalAst.TypeProvider.UnaryOperations[unaryOperatorAst.Operator.Type];
		var newValue = unaryOperation( literalAst.Value );
		var newToken = new Token( TokenType.Literal, newValue!, literalAst.StartLocation );
		return AddChange( new LiteralAst( newToken, literalAst.TypeProvider ) );
	}

	protected override Ast VisitIf( IfAst ifAst )
	{
		var newBooleanExpression = Visit( ifAst.BooleanExpressionAst );
		var falseBranch = Visit( ifAst.FalseBodyAst );
		
		if ( newBooleanExpression is LiteralAst literalAst && !(bool)literalAst.Value )
			return AddChange( falseBranch );
		
		var trueBranch = Visit( ifAst.TrueBodyAst );
		if ( trueBranch is NoOperationAst && falseBranch is NoOperationAst )
			return AddChange( new NoOperationAst( ifAst.StartLocation ) );
		
		return new IfAst( ifAst.StartLocation, newBooleanExpression, (BlockAst)trueBranch, falseBranch );
	}

	protected override Ast VisitFor( ForAst forAst )
	{
		var newBooleanExpression = Visit( forAst.BooleanExpressionAst );
		if ( newBooleanExpression is LiteralAst literalAst && !(bool)literalAst.Value )
			return AddChange( new NoOperationAst( forAst.StartLocation ) );
		
		var newBlock = Visit( forAst.BodyAst );
		if ( newBlock is NoOperationAst )
			return AddChange( new NoOperationAst( forAst.StartLocation ) );

		var newIterator = Visit( forAst.IteratorAst );
		return new ForAst( forAst.StartLocation, forAst.VariableDeclarationAst, newBooleanExpression,
			(AssignmentAst)newIterator, (BlockAst)newBlock );
	}

	protected override Ast VisitWhile( WhileAst whileAst )
	{
		var newBooleanExpression = Visit( whileAst.BooleanExpressionAst );
		if ( newBooleanExpression is LiteralAst literalAst && !(bool)literalAst.Value )
			return AddChange( new NoOperationAst( whileAst.StartLocation ) );
		
		var newBlock = Visit( whileAst.BodyAst );
		return newBlock is NoOperationAst
			? AddChange( new NoOperationAst( whileAst.StartLocation ) )
			: new WhileAst( whileAst.StartLocation, newBooleanExpression, (BlockAst)newBlock );
	}

	protected override Ast VisitDoWhile( DoWhileAst doWhileAst )
	{
		var newBooleanExpression = Visit( doWhileAst.BooleanExpressionAst );
		var newBlock = Visit( doWhileAst.BodyAst );

		if ( newBooleanExpression is LiteralAst literalAst && !(bool)literalAst.Value )
			return AddChange( newBlock );
		
		return newBlock is NoOperationAst
			? AddChange( new NoOperationAst( doWhileAst.StartLocation ) )
			: new DoWhileAst( doWhileAst.StartLocation, newBooleanExpression, (BlockAst)newBlock );
	}

	// If method is unused and not global. Remove it.
	protected override Ast VisitMethodDeclaration( MethodDeclarationAst methodDeclarationAst )
	{
		foreach ( var parameter in methodDeclarationAst.ParameterAsts )
			Visit( parameter );
		
		var newScope = Visit( methodDeclarationAst.BodyAst );
		if ( newScope is not NoOperationAst )
		{
			return new MethodDeclarationAst( methodDeclarationAst.ReturnTypeAst,
				methodDeclarationAst.MethodNameAst, methodDeclarationAst.ParameterAsts,
				(BlockAst)newScope );
		}

		_removedMethods.Current.AddOrUpdate( MethodSignature.From( methodDeclarationAst ), true );
		return AddChange( new NoOperationAst( methodDeclarationAst.StartLocation ) );
	}

	// If method can resolve to a literal, create one.
	// If method is unused or undefined due to previous optimization. Remove it.
	protected override Ast VisitMethodCall( MethodCallAst methodCallAst )
	{
		if ( _removedMethods.Current.ContainsKey( MethodSignature.From( methodCallAst ) ) )
			return AddChange( new NoOperationAst( methodCallAst.StartLocation ) );

		var newArguments = ImmutableArray.CreateBuilder<Ast>();
		foreach ( var argument in methodCallAst.ArgumentAsts )
			newArguments.Add( Visit( argument ) );

		return new MethodCallAst( methodCallAst.NameToken, newArguments.ToImmutable(), methodCallAst.ArgumentTypes );
	}

	protected override Ast VisitParameter( ParameterAst parameterAst )
	{
		return parameterAst;
	}

	// If variable is only set and never gotten while not being a global. Remove it.
	protected override Ast VisitVariableDeclaration( VariableDeclarationAst variableDeclarationAst ) =>
		new VariableDeclarationAst( variableDeclarationAst.VariableTypeAst, variableDeclarationAst.VariableNameAsts,
			Visit( variableDeclarationAst.DefaultExpressionAst ) );

	// If variable is unused or undefined due to previous optimization. Remove it.
	protected override Ast VisitVariable( VariableAst variableAst )
	{
		return variableAst;
	}

	protected override Ast VisitVariableType( VariableTypeAst variableTypeAst )
	{
		return variableTypeAst;
	}

	protected override Ast VisitLiteral( LiteralAst literalAst )
	{
		return literalAst;
	}

	protected override Ast VisitNoOperation( NoOperationAst noOperationAst )
	{
		return noOperationAst;
	}

	protected override Ast VisitComment( CommentAst commentAst )
	{
		return new NoOperationAst( commentAst.StartLocation );
	}

	protected override Ast VisitWhitespace( WhitespaceAst whitespaceAst )
	{
		return new NoOperationAst( whitespaceAst.StartLocation );
	}

	public static Ast Optimize( Ast ast ) => Optimize( ast, out _ );
	public static Ast Optimize( Ast ast, out StageDiagnostics diagnostics )
	{
		var optimizer = new Optimizer();
		var optimizedAst = optimizer.OptimizeTree( ast );
		diagnostics = optimizer._diagnostics;
		return optimizedAst;
	}
}
