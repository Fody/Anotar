using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetLogEnabled(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "get_IsTraceEnabled")
        {
            return isTraceEnabledMethod;
        }
        if (name == "get_IsDebugEnabled")
        {
            return isDebugEnabledMethod;
        }
        if (name == "get_IsInformationEnabled")
        {
            return isInformationEnabledMethod;
        }
        if (name == "get_IsWarningEnabled")
        {
            return isWarningEnabledMethod;
        }
        if (name == "get_IsErrorEnabled")
        {
            return isErrorEnabledMethod;
        }
        if (name == "get_IsFatalEnabled")
        {
            return isFatalEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetLogEnabledForLog(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Trace")
        {
            return isTraceEnabledMethod;
        }
        if (name == "Debug")
        {
            return isDebugEnabledMethod;
        }
        if (name == "Information")
        {
            return isInformationEnabledMethod;
        }
        if (name == "Warning")
        {
            return isWarningEnabledMethod;
        }
        if (name == "Error")
        {
            return isErrorEnabledMethod;
        }
        if (name == "Fatal")
        {
            return isFatalEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetNormalOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Trace")
        {
            return TraceMethod;
        }
        if (name == "Debug")
        {
            return DebugMethod;
        }
        if (name == "Information")
        {
            return InformationMethod;
        }
        if (name == "Warning")
        {
            return WarningMethod;
        }
        if (name == "Error")
        {
            return ErrorMethod;
        }
        if (name == "Fatal")
        {
            return FatalMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetExceptionOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Trace")
        {
            return TraceExceptionMethod;
        }
        if (name == "Debug")
        {
            return DebugExceptionMethod;
        }
        if (name == "Information")
        {
            return InformationExceptionMethod;
        }
        if (name == "Warning")
        {
            return WarningExceptionMethod;
        }
        if (name == "Error")
        {
            return ErrorExceptionMethod;
        }
        if (name == "Fatal")
        {
            return FatalExceptionMethod;
        }
        throw new Exception("Invalid method name");
    }
}