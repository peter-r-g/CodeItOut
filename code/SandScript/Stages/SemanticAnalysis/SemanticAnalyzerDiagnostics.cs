using System;

namespace SandScript;

public sealed class SemanticAnalyzerDiagnostics : StageDiagnostics
{
	protected override string StageName => "Semantic analysis";

	public void TypeMismatch( ITypeProvider expectedType, ITypeProvider gotType, TokenLocation location )
	{
		Error( $"Expected {expectedType}, got {gotType}", location );
	}

	public void Undefined( string name )
	{
		Error( $"{name} is not defined" );
	}

	public void Redefined( string name, Guid containerGuid )
	{
		Error( $"{name} is already defined in {containerGuid}" );
	}

	public void Unreadable( string name )
	{
		Error( $"{name} is not readable" );
	}

	public void Unwritable( string name )
	{
		Error( $"{name} is not writable" );
	}

	public void UnsupportedBinaryOperatorForType( TokenType operatorType, ITypeProvider type,
		TokenLocation location )
	{
		Error( $"Binary operator {operatorType} not supported for type \"{type}\"", location );
	}

	public void UnsupportedUnaryOperatorForType( TokenType operatorType, ITypeProvider type,
		TokenLocation location )
	{
		Error( $"Unary operator {operatorType} not supported for type \"{type}\"", location );
	}

	public void ArgumentCountMismatch( int expectedArgumentAmount, int gotArgumentAmount, TokenLocation location )
	{
		Error( $"Expected {expectedArgumentAmount} argument(s), got {gotArgumentAmount}", location );
	}

	public void MissingParameter( string parameterName, string methodName, TokenLocation location )
	{
		Error( $"The parameter \"{parameterName}\" of the \"{methodName}\" method was not provided", location );
	}

	public void MissingType( TokenLocation location )
	{
		Error( "Expected type in initial value", location );
	}
}
