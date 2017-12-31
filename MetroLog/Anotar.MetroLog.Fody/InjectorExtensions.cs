using System;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public MethodReference GetLogEnabledForLog(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Trace" || name == "TraceException")
        {
            return IsTraceEnabledMethod;
        }
        if (name == "Debug" || name == "DebugException")
        {
            return IsDebugEnabledMethod;
        }
        if (name == "Info" || name == "InfoException")
        {
            return IsInfoEnabledMethod;
        }
        if (name == "Warn" || name == "WarnException")
        {
            return IsWarnEnabledMethod;
        }
        if (name == "Error" || name == "ErrorException")
        {
            return IsErrorEnabledMethod;
        }
        if (name == "Fatal" || name == "FatalException")
        {
            return IsFatalEnabledMethod;
        }
        throw new Exception("Invalid method name");
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
        throw new Exception("Invalid method name");
    }

    public MethodReference GetExceptionOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "TraceException" || name == "Trace")
        {
            return TraceExceptionMethod;
        }
        if (name == "DebugException" || name == "Debug")
        {
            return DebugExceptionMethod;
        }
        if (name == "InfoException" || name == "Info")
        {
            return InfoExceptionMethod;
        }
        if (name == "WarnException" || name == "Warn")
        {
            return WarnExceptionMethod;
        }
        if (name == "ErrorException" || name == "Error")
        {
            return ErrorExceptionMethod;
        }
        if (name == "FatalException" || name == "Fatal")
        {
            return FatalExceptionMethod;
        }
        throw new Exception("Invalid method name");
    }
}