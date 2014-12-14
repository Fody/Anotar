using System;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public int GetLogEvent(MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return DebugLogEvent;
        }
        if (methodReference.Name == "Info")
        {
            return InfoLogEvent;
        }
        if (methodReference.Name == "Warning")
        {
            return WarningLogEvent;
        }
        if (methodReference.Name == "Error")
        {
            return ErrorLogEvent;
        }
        throw new Exception("Invalid method name");
    }

}