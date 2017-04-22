using Mono.Cecil;

public class AttributeFinder
{
    public AttributeFinder(MethodDefinition method)
    {
        var customAttributes = method.CustomAttributes;
        if (customAttributes.ContainsAttribute("Anotar.Catel.LogToDebugOnExceptionAttribute"))
        {
            FoundDebug = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Catel.LogToInfoOnExceptionAttribute"))
        {
            FoundInfo = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Catel.LogToWarningOnExceptionAttribute"))
        {
            FoundWarning = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Catel.LogToErrorOnExceptionAttribute"))
        {
            FoundError = true;
            Found = true;
        }
    }

    public bool Found;
    public bool FoundInfo;
    public bool FoundDebug;
    public bool FoundWarning;
    public bool FoundError;
}