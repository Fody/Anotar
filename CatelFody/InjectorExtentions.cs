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
    public MethodReference GetLogEnabled(MethodReference methodReference)
    {
        if (methodReference.Name == "get_IsDebugEnabled")
        {
            return isDebugEnabledMethod;
        }
        if (methodReference.Name == "get_IsInfoEnabled")
        {
            return isInfoEnabledMethod;
        }
        if (methodReference.Name == "get_IsWarningEnabled")
        {
            return isWarningEnabledMethod;
        }
        if (methodReference.Name == "get_IsErrorEnabled")
        {
            return isErrorEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

}