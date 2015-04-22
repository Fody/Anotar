using System;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public int GetLogEvent(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Debug")
        {
            return DebugLogEvent;
        }
        if (name == "Info")
        {
            return InfoLogEvent;
        }
        if (name == "Warning")
        {
            return WarningLogEvent;
        }
        if (name == "Error")
        {
            return ErrorLogEvent;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetLogEnabledForLog(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Debug")
        {
            return isDebugEnabledMethod;
        }
        if (name == "Info")
        {
            return isInfoEnabledMethod;
        }
        if (name == "Warning")
        {
            return isWarningEnabledMethod;
        }
        if (name == "Error")
        {
            return isErrorEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetLogEnabledForIs(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "get_IsDebugEnabled")
        {
            return isDebugEnabledMethod;
        }
        if (name == "get_IsInfoEnabled")
        {
            return isInfoEnabledMethod;
        }
        if (name == "get_IsWarningEnabled")
        {
            return isWarningEnabledMethod;
        }
        if (name == "get_IsErrorEnabled")
        {
            return isErrorEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

}