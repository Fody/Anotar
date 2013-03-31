using Mono.Cecil;

public class AttributeFinder
{
	public AttributeFinder(MethodDefinition method)
	{
		var customAttributes = method.CustomAttributes;
		if (customAttributes.ContainsAttribute("LogToTraceOnExceptionAttribute"))
		{
			FoundTrace = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("LogToDebugOnExceptionAttribute"))
		{
			FoundDebug = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("LogToInfoOnExceptionAttribute"))
		{
			FoundInfo = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("LogToWarnOnExceptionAttribute"))
		{
			FoundWarn = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("LogToErrorOnExceptionAttribute"))
		{
			FoundError = true;
			Found = true;
		}

	}

	public bool Found;

	public bool FoundInfo;

	public bool FoundDebug;

	public bool FoundWarn;

	public bool FoundError;

	public bool FoundTrace;
}