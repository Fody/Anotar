using Mono.Cecil;

public class AttributeFinder
{
    public AttributeFinder(MethodDefinition method)
    {
        var customAttributes = method.CustomAttributes;
        if (customAttributes.ContainsAttribute("Anotar.MetroLog.LogToTraceOnExceptionAttribute"))
        {
            FoundTrace = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.MetroLog.LogToDebugOnExceptionAttribute"))
        {
            FoundDebug = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.MetroLog.LogToInfoOnExceptionAttribute"))
        {
            FoundInfo = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.MetroLog.LogToWarnOnExceptionAttribute"))
        {
            FoundWarn = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.MetroLog.LogToErrorOnExceptionAttribute"))
        {
            FoundError = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.MetroLog.LogToFatalOnExceptionAttribute"))
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