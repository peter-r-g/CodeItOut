namespace SandScript;

public enum TokenType
{
	None,
	
	Literal,
	Identifier,
	
	If,
	Else,
	Do,
	While,
	For,
	Return,
	
	Whitespace,
	Comment,
	MultiLineComment,

	AmpersandAmpersand,
	Asterisk,
	AsteriskEquals,
	Bang,
	BangEquals,
	Comma,
	Equals,
	EqualsEquals,
	GreaterThan,
	GreaterThanEquals,
	Hat,
	LeftCurlyBracket,
	LeftParenthesis,
	LeftSquareBracket,
	LessThan,
	LessThanEquals,
	Minus,
	MinusEquals,
	Percent,
	PercentEquals,
	Period,
	PipePipe,
	Plus,
	PlusEquals,
	RightParenthesis,
	RightCurlyBracket,
	RightSquareBracket,
	SemiColon,
	Slash,
	SlashEquals,

	Eof
}
