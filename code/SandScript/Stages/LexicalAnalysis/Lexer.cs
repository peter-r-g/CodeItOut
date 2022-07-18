using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace SandScript;

public sealed class Lexer
{
	public readonly LexerDiagnostics Diagnostics = new();

	public readonly string Text;
	private readonly bool _lexNonEssentialTokens;

	internal char CurrentChar;
	internal bool IsCurrentEof;
	
	internal int Position;
	internal int Row = 1;
	internal int Column = 1;

	private Lexer( string text, bool lexNonEssentialTokens )
	{
		if ( string.IsNullOrWhiteSpace( text ) )
		{
			Diagnostics.NoCode();
			Text = string.Empty;
			return;
		}

		Position = 0;
		Row = 1;
		Column = 1;
	    
		Text = text;
		_lexNonEssentialTokens = lexNonEssentialTokens;
		CurrentChar = text[Position];
		IsCurrentEof = CurrentChar == '\0';
	}

	public void Advance()
    {
	    if ( CurrentChar == '\n' )
	    {
		    Row++;
		    Column = 1;
	    }
	    
	    Position++;
	    if ( Position > Text.Length - 1 )
	    {
		    CurrentChar = '\0';
	    }
	    else
	    {
		    CurrentChar = Text[Position];
		    Column++;
	    }

	    IsCurrentEof = CurrentChar == '\0';
    }
    
    public char Peek( int count = 1 )
    {
	    var peekPosition = Position + count;
	    return peekPosition > Text.Length - 1 ? '\0' : Text[peekPosition];
    }
    
    private Token GetNextToken()
    {
	    while ( !IsCurrentEof )
	    {
		    if ( char.IsWhiteSpace( CurrentChar ) )
		    {
			    var whitespaceToken = GetWhitespaceToken();
			    if ( _lexNonEssentialTokens )
				    return whitespaceToken;

			    continue;
		    }

		    if ( CurrentChar == '/' )
		    {
			    switch ( Peek() )
			    {
				    case '/':
					    var commentToken = GetCommentToken();
					    if ( _lexNonEssentialTokens )
						    return commentToken;

					    continue;
				    case '*':
					    var multiLineCommentToken = GetMultiLineCommentToken();
					    if ( _lexNonEssentialTokens )
						    return multiLineCommentToken;

					    continue;
			    }
		    }

		    if ( TryGetLiteral( out var token ) )
			    return token;

		    if ( CurrentChar == '_' || char.IsLetterOrDigit( CurrentChar ) )
			    return GetIdentifierOrKeyword();

		    if ( TryGetMiscToken( 2, out token ) )
			    return token;

		    if ( TryGetMiscToken( 1, out token ) )
			    return token;

		    var location = new TokenLocation( Row, Column );
		    Diagnostics.UnknownToken( CurrentChar, location );
		    return new Token( TokenType.None, CurrentChar, location );
	    }

	    return new Token( TokenType.Eof, string.Empty, Row, Column );
    }

    private Token GetWhitespaceToken()
    {
	    var row = Row;
	    var column = Column;

	    var startPos = Position;
	    while ( !IsCurrentEof && char.IsWhiteSpace( CurrentChar ) )
			Advance();

	    return new Token( TokenType.Whitespace, Text.Substring( startPos, Position - startPos ), row, column );
    }

    private Token GetCommentToken()
    {
	    var row = Row;
	    var column = Column;
	    
	    Advance();
	    Advance();

	    var startPos = Position;
	    while ( CurrentChar != '\n' && !IsCurrentEof )
		    Advance();
	    var comment = Text.Substring( startPos, Position - startPos ).Trim();

	    Advance();

	    return new Token( TokenType.Comment, comment, row, column );
    }

    private Token GetMultiLineCommentToken()
    {
	    var row = Row;
	    var column = Column;
	    
	    Advance();
	    Advance();

	    var startPos = Position;
	    while ( !(CurrentChar == '*' && Peek() == '/') )
		    Advance();
	    var comment = Text.Substring( startPos, Position - startPos ).Trim();

	    Advance();
	    Advance();

	    return new Token( TokenType.MultiLineComment, comment, row, column );
    }

    private bool TryGetLiteral( [NotNullWhen( true )] out Token? token )
    {
	    foreach ( var typeProvider in TypeProviders.GetAll() )
	    {
		    if ( typeProvider is not ILiteralTypeProvider literalTypeProvider )
			    continue;

		    var row = Row;
		    var column = Column;
		    var literal = literalTypeProvider.GetLiteral( this );
		    if ( literal is null )
			    continue;

		    token = new Token( TokenType.Literal, literal, row, column );
		    return true;
	    }

	    token = null;
	    return false;
    }

    private Token GetIdentifierOrKeyword()
    {
	    var row = Row;
	    var column = Column;
	    
	    var startPos = Position;
	    while ( !IsCurrentEof && (CurrentChar == '_' || char.IsLetterOrDigit( CurrentChar )) )
		    Advance();

	    var str = Text.Substring( startPos, Position - startPos );
	    return TokenTypeExtension.TryParseKeyword( str, out var token )
		    ? new Token( token.Value, str, row, column )
		    : new Token( TokenType.Identifier, str, row, column );
    }

    private bool TryGetMiscToken( int numCharacters, [NotNullWhen(true)] out Token? token )
    {
	    if ( IsCurrentEof )
	    {
		    token = null;
		    return false;
	    }

	    var row = Row;
	    var column = Column;

	    if ( Peek( numCharacters-1 ) == '\0' )
	    {
		    token = null;
		    return false;
	    }

	    var str = Text.Substring( Position, numCharacters );
	    if ( !TokenTypeExtension.TryParseToken( str, out var tokenType ) )
	    {
		    token = null;
		    return false;
	    }
	    
	    for ( var i = 0; i < numCharacters; i++ )
		    Advance();

	    token = new Token( tokenType.Value, str, row, column );
	    return true;
    }

    public static ImmutableArray<Token> Lex( string text, bool lexNonEssentialTokens = false ) =>
	    Lex( text, lexNonEssentialTokens, out _ );
    public static ImmutableArray<Token> Lex( string text, bool lexNonEssentialTokens,
	    out StageDiagnostics diagnostics )
    {
	    var lexer = new Lexer( text, lexNonEssentialTokens );
	    var tokens = ImmutableArray.CreateBuilder<Token>();

	    do
	    {
		    tokens.Add( lexer.GetNextToken() );
	    } while ( tokens[^1].Type != TokenType.Eof );

	    diagnostics = lexer.Diagnostics;
	    return tokens.ToImmutable();
    }
}
