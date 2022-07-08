using System;
using System.Reflection;

namespace SandScript.Exceptions;

public sealed class UnreadableVariableException : Exception
{
	public MemberInfo UnreadableMember;
	public ScriptVariableAttribute VariableAttribute;

	public UnreadableVariableException( MemberInfo memberInfo, ScriptVariableAttribute variableAttribute )
		: base( "The property \"" + memberInfo.Name + "\" is unreadable" )
	{
		UnreadableMember = memberInfo;
		VariableAttribute = variableAttribute;
	}
}
