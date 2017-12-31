using Mono.Cecil;

public class AttributeFinder
{
    public AttributeFinder(MethodDefinition method)
    {
        var customAttributes = method.CustomAttributes;
        if (customAttributes.ContainsAttribute("Anotar.NLog.LogToTraceOnExceptionAttribute"))
        {
            FoundTrace = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NLog.LogToDebugOnExceptionAttribute"))
        {
            FoundDebug = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NLog.LogToInfoOnExceptionAttribute"))
        {
            FoundInfo = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NLog.LogToWarnOnExceptionAttribute"))
        {
            FoundWarn = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NLog.LogToErrorOnExceptionAttribute"))
        {
            FoundError = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NLog.LogToFatalOnExceptionAttribute"))
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