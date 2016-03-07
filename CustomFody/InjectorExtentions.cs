using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetLogEnabled(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "get_IsTraceEnabled")
        {
            return IsTraceEnabledMethod;
        }
        if (name == "get_IsDebugEnabled")
        {
            return IsDebugEnabledMethod;
        }
        if (name == "get_IsInformationEnabled")
        {
            return IsInformationEnabledMethod;
        }
        if (name == "get_IsWarningEnabled")
        {
            return IsWarningEnabledMethod;
        }
        if (name == "get_IsErrorEnabled")
        {
            return IsErrorEnabledMethod;
        }
        if (name == "get_IsFatalEnabled")
        {
            return IsFatalEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetLogEnabledForLog(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Trace")
        {
            return IsTraceEnabledMethod;
        }
        if (name == "Debug")
        {
            return IsDebugEnabledMethod;
        }
        if (name == "Information")
        {
            return IsInformationEnabledMethod;
        }
        if (name == "Warning")
        {
            return IsWarningEnabledMethod;
        }
        if (name == "Error")
        {
            return IsErrorEnabledMethod;
        }
        if (name == "Fatal")
        {
            return IsFatalEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetFormatOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Trace")
        {
            return TraceFormatMethod;
        }
        if (name == "Debug")
        {
            return DebugFormatMethod;
        }
        if (name == "Information")
        {
            return InformationFormatMethod;
        }
        if (name == "Warning")
        {
            return WarningFormatMethod;
        }
        if (name == "Error")
        {
            return ErrorFormatMethod;
        }
        if (name == "Fatal")
        {
            return FatalFormatMethod;
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