using System.Collections.Immutable;
using SandScript.AbstractSyntaxTrees;

namespace SandScript;

public sealed class Parser
{
	private readonly ImmutableArray<Token> _tokens;
	private int _tokenPosition;

	private Token CurrentToken => PeekToken( 0 );
	
	private readonly ParserDiagnostics _diagnostics = new();

	private Parser( ImmutableArray<Token> tokens )
	{
		_tokens = tokens;
	}

	private ProgramAst Parse()
	{
		var statements = StatementList();
		if ( CurrentToken.Type != TokenType.Eof )
			_diagnostics.UnexpectedToken( TokenType.Eof, CurrentToken.Type, CurrentToken.Location );

		return new ProgramAst( statements );
	}
	
	private Token PeekToken( int offset )
	{
		if ( offset == 0 )
			return _tokens[_tokenPosition];
		
		var position = _tokenPosition + offset;
		if ( position < 0 )
			return _tokens[0];
		
		return position > _tokens.Length ? _tokens[^1] : _tokens[position];
	}

	private Token NextToken()
	{
		var token = CurrentToken;
		_tokenPosition++;
		return token;
	}

	private void EatToken( TokenType type )
	{
		if ( CurrentToken.Type != type )
		{
			_diagnostics.UnexpectedToken( type, CurrentToken.Type, CurrentToken.Location );
			return;
		}

		NextToken();
	}

	private BlockAst BlockStatement()
	{
		var startLocation = CurrentToken.Location;
		if ( CurrentToken.Type != TokenType.LeftCurlyBracket )
			return new BlockAst( startLocation, ImmutableArray.Create( Statement() ) );
		
		EatToken( TokenType.LeftCurlyBracket );
		var statements = ImmutableArray<Ast>.Empty;
		if ( CurrentToken.Type != TokenType.RightCurlyBracket )
			statements = StatementList();
		EatToken( TokenType.RightCurlyBracket );

		return new BlockAst( startLocation, statements );
	}
	
	private ImmutableArray<Ast> StatementList()
	{
		var statements = ImmutableArray.CreateBuilder<Ast>();

		while ( CurrentToken.Type != TokenType.Eof && CurrentToken.Type != TokenType.RightCurlyBracket )
		{
			statements.Add( Statement() );
			if ( CurrentToken.Type == TokenType.SemiColon )
				EatToken( TokenType.SemiColon );
		}

		return statements.ToImmutable();
	}

	private Ast Statement()
	{
		return CurrentToken.Type switch
		{
			TokenType.If => IfStatement(),
			TokenType.For => ForStatement(),
			TokenType.Do => DoWhileStatement(),
			TokenType.While => WhileStatement(),
			TokenType.Return => ReturnStatement(),
			
			TokenType.Identifier when PeekToken( 1 ).Type == TokenType.LeftParenthesis => MethodCallStatement(),
			TokenType.Identifier when PeekToken( 1 ).Type == TokenType.Identifier &&
			                          PeekToken( 2 ).Type == TokenType.LeftParenthesis => MethodDeclarationStatement(),
			TokenType.Identifier when PeekToken( 1 ).Type == TokenType.Identifier => VariableDeclarationStatement( true ),
			TokenType.Identifier => AssignmentStatement(),
			
			TokenType.LeftCurlyBracket => BlockStatement(),
			
			TokenType.Whitespace => Whitespace(),
			TokenType.Comment => Comment(),
			
			_ => SkipToken()
		};
	}
	
	private ReturnAst ReturnStatement()
	{
		var startLocation = CurrentToken.Location;
		EatToken( TokenType.Return );
		
		var returnExpression = Empty();
		if ( CurrentToken.Type != TokenType.SemiColon )
			returnExpression = Expression();

		return new ReturnAst( startLocation, returnExpression );
	}

	private IfAst IfStatement()
	{
		var startLocation = CurrentToken.Location;
		EatToken( TokenType.If );
		EatToken( TokenType.LeftParenthesis );
		var expression = Expression();
		EatToken( TokenType.RightParenthesis );
		
		var trueBlock = BlockStatement();
		if ( CurrentToken.Type == TokenType.SemiColon )
			EatToken( TokenType.SemiColon );
		
		var falseBlock = Empty();
		if ( CurrentToken.Type != TokenType.Else )
			return new IfAst( startLocation, expression, trueBlock, falseBlock );

		EatToken( TokenType.Else );
		return new IfAst( startLocation, expression, trueBlock, BlockStatement() );
	}

	private ForAst ForStatement()
	{
		var startLocation = CurrentToken.Location;
		EatToken( TokenType.For );
		EatToken( TokenType.LeftParenthesis );
		var forVar = VariableDeclarationStatement( false );
		EatToken( TokenType.SemiColon );
		var booleanExpression = Expression();
		EatToken( TokenType.SemiColon );
		var iterator = AssignmentStatement();
		EatToken( TokenType.RightParenthesis );

		return new ForAst( startLocation, forVar, booleanExpression, iterator, BlockStatement() );
	}
	
	private WhileAst WhileStatement()
	{
		var startLocation = CurrentToken.Location;
		EatToken( TokenType.While );
		EatToken( TokenType.LeftParenthesis );
		var expression = Expression();
		EatToken( TokenType.RightParenthesis );
		
		return new WhileAst( startLocation, expression, BlockStatement() );
	}

	private DoWhileAst DoWhileStatement()
	{
		var startLocation = CurrentToken.Location;
		EatToken( TokenType.Do );
		var block = BlockStatement();
		
		EatToken( TokenType.While );
		EatToken( TokenType.LeftParenthesis );
		var expression = Expression();
		EatToken( TokenType.RightParenthesis );

		return new DoWhileAst( startLocation, expression, block );
	}
	
	private MethodDeclarationAst MethodDeclarationStatement()
	{
		var returnType = VariableType();
		var methodName = Variable();
		
		EatToken( TokenType.LeftParenthesis );
		var parameters = ImmutableArray.CreateBuilder<ParameterAst>();
		if ( CurrentToken.Type != TokenType.RightParenthesis )
		{
			parameters.Add( Parameter() );
			while ( CurrentToken.Type == TokenType.Comma )
			{
				EatToken( TokenType.Comma );
				parameters.Add( Parameter() );
			}
		}
		EatToken( TokenType.RightParenthesis );

		return new MethodDeclarationAst( returnType, methodName, parameters.ToImmutable(), BlockStatement() );
	}

	private ParameterAst Parameter()
	{
		var parameterType = VariableType();
		var parameterName = Variable();

		return new ParameterAst( parameterType, parameterName );
	}
	
	private MethodCallAst MethodCallStatement()
	{
		var methodNameToken = CurrentToken;
		EatToken( TokenType.Identifier );
		EatToken( TokenType.LeftParenthesis );

		var parameters = ImmutableArray.CreateBuilder<Ast>();
		if ( CurrentToken.Type != TokenType.RightParenthesis )
			parameters.Add( Expression() );

		while ( CurrentToken.Type == TokenType.Comma )
		{
			EatToken( TokenType.Comma );
			parameters.Add( Expression() );
		}
		
		EatToken( TokenType.RightParenthesis );
		return new MethodCallAst( methodNameToken, parameters.ToImmutable() );
	}

	private VariableDeclarationAst VariableDeclarationStatement( bool allowManyNameTokens )
	{
		var variableType = VariableType();
		var variableNameTokens = ImmutableArray.CreateBuilder<VariableAst>();
		variableNameTokens.Add( Variable() );

		if ( allowManyNameTokens )
		{
			while ( CurrentToken.Type == TokenType.Comma )
			{
				EatToken( TokenType.Comma );
				variableNameTokens.Add( Variable() );
			}	
		}

		var defaultValue = Empty();
		if ( CurrentToken.Type != TokenType.Equals )
			return new VariableDeclarationAst( variableType, variableNameTokens.ToImmutable(), defaultValue );

		EatToken( TokenType.Equals );
		defaultValue = Expression();

		return new VariableDeclarationAst( variableType, variableNameTokens.ToImmutable(), defaultValue );
	}
	
	private VariableAst Variable()
	{
		var variableToken = CurrentToken;
		EatToken( TokenType.Identifier );

		return new VariableAst( variableToken );
	}

	private VariableTypeAst VariableType()
	{
		var typeToken = CurrentToken;
		
		if ( TypeProviders.GetByIdentifier( (string)typeToken.Value ) is not null )
			NextToken();
		else
			_diagnostics.UnknownVariableType( typeToken );

		return new VariableTypeAst( typeToken );
	}

	private AssignmentAst AssignmentStatement()
	{
		var left = Variable();
		var assignToken = CurrentToken;

		if ( !assignToken.Type.IsAssignmentOperator() )
			_diagnostics.UnknownAssignmentOperator( assignToken );

		NextToken();
		return new AssignmentAst( left, assignToken, Expression() );
	}

	private Ast Expression( int parentPrecedence = int.MaxValue )
	{
		Ast left;
		var unaryOperatorPrecedence = CurrentToken.Type.GetUnaryOperatorPrecedence();
		
		if ( unaryOperatorPrecedence != int.MaxValue && unaryOperatorPrecedence < parentPrecedence)
		{
			var op = NextToken();
			var operand = Expression( unaryOperatorPrecedence );
			left = new UnaryOperatorAst(op, operand);
		}
		else
		{
			left = PrimaryExpression();
		}

		while ( true )
		{
			var precedence = CurrentToken.Type.GetBinaryOperatorPrecedence();
			if ( precedence == int.MaxValue || precedence > parentPrecedence )
				break;

			var op = NextToken();
			var right = Expression( precedence );
			left = new BinaryOperatorAst( left, op, right);
		}

		return left;
	}
	
	private Ast PrimaryExpression()
	{
		var currentToken = CurrentToken;
		
		switch ( currentToken.Type )
		{
			case TokenType.LeftParenthesis:
				EatToken( TokenType.LeftParenthesis );
				var expression = Expression();
				EatToken( TokenType.RightParenthesis );
				return expression;
			case TokenType.Literal:
				NextToken();
				foreach ( var typeProvider in TypeProviders.GetAll() )
				{
					if ( typeProvider is not ILiteralTypeProvider literalTypeProvider )
						continue;

					var literalAst = literalTypeProvider.GetLiteralAst( currentToken );
					if ( literalAst is not null )
						return literalAst;
				}

				_diagnostics.UnhandledLiteralToken( currentToken );
				return Empty();
			case TokenType.Identifier when PeekToken( 1 ).Type == TokenType.LeftParenthesis:
				return MethodCallStatement();
			case TokenType.Identifier:
				return Variable();
			default:
				_diagnostics.UnexpectedResult( "primary expression", currentToken );
				return SkipToken();
		}
	}

	private Ast Whitespace()
	{
		var ast = new WhitespaceAst( CurrentToken.Location, ((string)CurrentToken.Value).Length );
		EatToken( TokenType.Whitespace );

		return ast;
	}

	private Ast Comment()
	{
		var ast = new CommentAst( CurrentToken.Location, (string)CurrentToken.Value, ((string)CurrentToken.Value).Contains( '\n' ) );
		EatToken( TokenType.Comment );

		return ast;
	}

	private Ast Empty()
	{
		return new NoOperationAst( CurrentToken.Location );
	}

	private Ast SkipToken()
	{
		_diagnostics.UnexpectedResult( "statement", CurrentToken );
		var noop = new NoOperationAst( CurrentToken.Location );
		NextToken();
		return noop;
	}

	public static ProgramAst Parse( ImmutableArray<Token> tokens ) => Parse( tokens, out _ );
	public static ProgramAst Parse( ImmutableArray<Token> tokens, out StageDiagnostics diagnostics )
	{
		var parser = new Parser( tokens );
		var programAst = parser.Parse();
		diagnostics = parser._diagnostics;
		return programAst;
	}
}
