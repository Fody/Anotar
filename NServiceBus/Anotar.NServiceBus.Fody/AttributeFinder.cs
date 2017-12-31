using Mono.Cecil;

public class AttributeFinder
{
    public AttributeFinder(MethodDefinition method)
    {
        var customAttributes = method.CustomAttributes;
        if (customAttributes.ContainsAttribute("Anotar.NServiceBus.LogToDebugOnExceptionAttribute"))
        {
            FoundDebug = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NServiceBus.LogToInfoOnExceptionAttribute"))
        {
            FoundInfo = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NServiceBus.LogToWarnOnExceptionAttribute"))
        {
            FoundWarn = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NServiceBus.LogToErrorOnExceptionAttribute"))
        {
            FoundError = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.NServiceBus.LogToFatalOnExceptionAttribute"))
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