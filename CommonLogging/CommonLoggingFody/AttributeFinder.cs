using Mono.Cecil;

public class AttributeFinder
{
    public AttributeFinder(MethodDefinition method)
    {
        var customAttributes = method.CustomAttributes;
        if (customAttributes.ContainsAttribute("Anotar.CommonLogging.LogToDebugOnExceptionAttribute"))
        {
            FoundDebug = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.CommonLogging.LogToInfoOnExceptionAttribute"))
        {
            FoundInfo = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.CommonLogging.LogToWarnOnExceptionAttribute"))
        {
            FoundWarn = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.CommonLogging.LogToErrorOnExceptionAttribute"))
        {
            FoundError = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.CommonLogging.LogToFatalOnExceptionAttribute"))
        {
            FoundFatal = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.CommonLogging.LogToTraceOnExceptionAttribute"))
        {
            FoundTrace = true;
            Found = true;
        }

    }

    public bool Found;
    public bool FoundInfo;
    public bool FoundDebug;
    public bool FoundWarn;
    public bool FoundTrace;
    public bool FoundError;
    public bool FoundFatal;

}