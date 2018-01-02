using Mono.Cecil;

public class AttributeFinder
{
    public AttributeFinder(MethodDefinition method)
    {
        var customAttributes = method.CustomAttributes;
        if (customAttributes.ContainsAttribute("Anotar.Serilog.LogToVerboseOnExceptionAttribute"))
        {
            FoundVerbose = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Serilog.LogToDebugOnExceptionAttribute"))
        {
            FoundDebug = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Serilog.LogToInformationOnExceptionAttribute"))
        {
            FoundInfo = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Serilog.LogToWarningOnExceptionAttribute"))
        {
            FoundWarn = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Serilog.LogToErrorOnExceptionAttribute"))
        {
            FoundError = true;
            Found = true;
        }
        if (customAttributes.ContainsAttribute("Anotar.Serilog.LogToFatalOnExceptionAttribute"))
        {
            FoundFatal = true;
            Found = true;
        }

    }

    public bool Found;
    public bool FoundInfo;
    public bool FoundDebug;
    public bool FoundVerbose;
    public bool FoundWarn;
    public bool FoundError;
    public bool FoundFatal;
}