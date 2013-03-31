using Mono.Cecil;

public class AttributeFinder
{
	public AttributeFinder(MethodDefinition method)
	{
		var customAttributes = method.CustomAttributes;
		if (customAttributes.ContainsAttribute("Anotar.Log4Net.LogToDebugOnExceptionAttribute"))
		{
			FoundDebug = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("Anotar.Log4Net.LogToInfoOnExceptionAttribute"))
		{
			FoundInfo = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("Anotar.Log4Net.LogToWarnOnExceptionAttribute"))
		{
			FoundWarn = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("Anotar.Log4Net.LogToErrorOnExceptionAttribute"))
		{
			FoundError = true;
			Found = true;
		}
		if (customAttributes.ContainsAttribute("Anotar.Log4Net.LogToFatalOnExceptionAttribute"))
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

}