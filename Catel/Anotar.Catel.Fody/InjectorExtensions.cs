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
            return IsDebugEnabledMethod;
        }
        if (name == "Info")
        {
            return IsInfoEnabledMethod;
        }
        if (name == "Warning")
        {
            return IsWarningEnabledMethod;
        }
        if (name == "Error")
        {
            return IsErrorEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetLogEnabledForIs(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "get_IsDebugEnabled")
        {
            return IsDebugEnabledMethod;
        }
        if (name == "get_IsInfoEnabled")
        {
            return IsInfoEnabledMethod;
        }
        if (name == "get_IsWarningEnabled")
        {
            return IsWarningEnabledMethod;
        }
        if (name == "get_IsErrorEnabled")
        {
            return IsErrorEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }
}