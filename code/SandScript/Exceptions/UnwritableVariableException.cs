using System;
using System.Reflection;

namespace SandScript.Exceptions;

public sealed class UnwritableVariableException : Exception
{
	public MemberInfo UnwritableMember;
	public ScriptVariableAttribute VariableAttribute;

	public UnwritableVariableException( MemberInfo memberInfo, ScriptVariableAttribute variableAttribute )
		: base( "The property \"" + memberInfo.Name + "\" is unreadable" )
	{
		UnwritableMember = memberInfo;
		VariableAttribute = variableAttribute;
	}
}
