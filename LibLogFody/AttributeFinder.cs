using Mono.Cecil;

public class AttributeFinder
{
	public AttributeFinder(MethodDefinition method)
	{
		var customAttributes = method.CustomAttributes;
        if (customAttributes.ContainsAttribute("Anotar.LibLog.LogToTraceOnExceptionAttribute"))
		{
			FoundTrace = true;
			Found = true;
		}
        if (customAttributes.ContainsAttribute("Anotar.LibLog.LogToDebugOnExceptionAttribute"))
		{
			FoundDebug = true;
			Found = true;
		}
        if (customAttributes.ContainsAttribute("Anotar.LibLog.LogToInfoOnExceptionAttribute"))
		{
			FoundInfo = true;
			Found = true;
		}
        if (customAttributes.ContainsAttribute("Anotar.LibLog.LogToWarnOnExceptionAttribute"))
		{
			FoundWarn = true;
			Found = true;
		}
        if (customAttributes.ContainsAttribute("Anotar.LibLog.LogToErrorOnExceptionAttribute"))
		{
			FoundError = true;
			Found = true;
		}
        if (customAttributes.ContainsAttribute("Anotar.LibLog.LogToFatalOnExceptionAttribute"))
		{
			FoundFatal = true;
			Found = true;
		}

	}

	public bool Found;

	public bool FoundInfo;

	public bool FoundDebug;

	public bool FoundWarn;

	public bool FoundError;
    public bool FoundFatal;

	public bool FoundTrace;
}