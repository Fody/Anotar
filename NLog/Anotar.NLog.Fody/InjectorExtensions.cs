using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetLogEnabledForLog(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name is "Trace" or "TraceException")
        {
            return IsTraceEnabledMethod;
        }
        if (name is "Debug" or "DebugException")
        {
            return IsDebugEnabledMethod;
        }
        if (name is "Info" or "InfoException")
        {
            return IsInfoEnabledMethod;
        }
        if (name is "Warn" or "WarnException")
        {
            return IsWarnEnabledMethod;
        }
        if (name is "Error" or "ErrorException")
        {
            return IsErrorEnabledMethod;
        }
        if (name is "Fatal" or "FatalException")
        {
            return IsFatalEnabledMethod;
        }
        throw new("Invalid method name");
    }

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
        if (name == "get_IsInfoEnabled")
        {
            return IsInfoEnabledMethod;
        }
        if (name == "get_IsWarnEnabled")
        {
            return IsWarnEnabledMethod;
        }
        if (name == "get_IsErrorEnabled")
        {
            return IsErrorEnabledMethod;
        }
        if (name == "get_IsFatalEnabled")
        {
            return IsFatalEnabledMethod;
        }
        throw new("Invalid method name");
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
        if (name == "Info")
        {
            return InfoMethod;
        }
        if (name == "Warn")
        {
            return WarnMethod;
        }
        if (name == "Error")
        {
            return ErrorMethod;
        }
        if (name == "Fatal")
        {
            return FatalMethod;
        }
        throw new("Invalid method name");
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
        if (name == "Info")
        {
            return InfoFormatMethod;
        }
        if (name == "Warn")
        {
            return WarnFormatMethod;
        }
        if (name == "Error")
        {
            return ErrorFormatMethod;
        }
        if (name == "Fatal")
        {
            return FatalFormatMethod;
        }
        throw new("Invalid method name");
    }

    public MethodReference GetExceptionOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name is "Trace" or "TraceException")
        {
            return TraceExceptionMethod;
        }
        if (name is "Debug" or "DebugException")
        {
            return DebugExceptionMethod;
        }
        if (name is "Info" or "InfoException")
        {
            return InfoExceptionMethod;
        }
        if (name is "Warn" or "WarnException")
        {
            return WarnExceptionMethod;
        }
        if (name is "Error" or "ErrorException")
        {
            return ErrorExceptionMethod;
        }
        if (name is "Fatal" or "FatalException")
        {
            return FatalExceptionMethod;
        }
        throw new("Invalid method name");
    }
}