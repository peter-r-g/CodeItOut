using SandScript.AbstractSyntaxTrees;
using SandScript.Exceptions;

namespace SandScript;

public abstract class NodeVisitor<T>
{
	public T Visit( Ast node )
	{
		PreVisit( node );
		var result = node switch
		{
			ProgramAst p => VisitProgram( p ),
			BlockAst ns => VisitBlock( ns ),
			ReturnAst r => VisitReturn( r ),
			
			AssignmentAst a => VisitAssignment( a ),
			BinaryOperatorAst bo => VisitBinaryOperator( bo ),
			UnaryOperatorAst uo => VisitUnaryOperator( uo ),
			
			IfAst i => VisitIf( i ),
			ForAst f => VisitFor( f ),
			WhileAst w => VisitWhile( w ),
			DoWhileAst dw => VisitDoWhile( dw ),
			
			MethodDeclarationAst pd => VisitMethodDeclaration( pd ),
			MethodCallAst pc => VisitMethodCall( pc ),
			ParameterAst p => VisitParameter( p ),
			
			VariableDeclarationAst vd => VisitVariableDeclaration( vd ),
			VariableAst v => VisitVariable( v ),
			VariableTypeAst vt => VisitVariableType( vt ),
			
			LiteralAst c => VisitLiteral( c ),
			NoOperationAst no => VisitNoOperation( no ),
			
			CommentAst c => VisitComment( c ),
			WhitespaceAst w => VisitWhitespace( w ),
			
			_ => throw new NoVisitHandlerException( node )
		};
		PostVisit( node, result );

		return result;
	}

	protected virtual void PreVisit( Ast node )
	{
	}

	protected virtual void PostVisit( Ast node, T result )
	{
	}

	protected abstract T VisitProgram( ProgramAst programAst );
	protected abstract T VisitBlock( BlockAst blockAst );
	protected abstract T VisitReturn( ReturnAst returnAst );
	
	protected abstract T VisitAssignment( AssignmentAst assignmentAst );
	protected abstract T VisitBinaryOperator( BinaryOperatorAst binaryOperatorAst );
	protected abstract T VisitUnaryOperator( UnaryOperatorAst unaryOperatorAst );

	protected abstract T VisitIf( IfAst ifAst );
	protected abstract T VisitFor( ForAst forAst );
	protected abstract T VisitWhile( WhileAst whileAst );
	protected abstract T VisitDoWhile( DoWhileAst doWhileAst );

	protected abstract T VisitMethodDeclaration( MethodDeclarationAst methodDeclarationAst );
	protected abstract T VisitMethodCall( MethodCallAst methodCallAst );
	protected abstract T VisitParameter( ParameterAst parameterAst );

	protected abstract T VisitVariableDeclaration( VariableDeclarationAst variableDeclarationAst );
	protected abstract T VisitVariable( VariableAst variableAst );
	protected abstract T VisitVariableType( VariableTypeAst variableTypeAst );

	protected abstract T VisitLiteral( LiteralAst literalAst );
	
	protected abstract T VisitNoOperation( NoOperationAst noOperationAst );

	protected abstract T VisitComment( CommentAst commentAst );
	protected abstract T VisitWhitespace( WhitespaceAst whitespaceAst );
}
