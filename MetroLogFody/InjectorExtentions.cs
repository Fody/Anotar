using System;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public MethodReference GetLogEnabledForLog(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Trace" || name == "TraceException")
        {
            return isTraceEnabledMethod;
        }
        if (name == "Debug" || name == "DebugException")
        {
            return isDebugEnabledMethod;
        }
        if (name == "Info" || name == "InfoException")
        {
            return isInfoEnabledMethod;
        }
        if (name == "Warn" || name == "WarnException")
        {
            return isWarnEnabledMethod;
        }
        if (name == "Error" || name == "ErrorException")
        {
            return isErrorEnabledMethod;
        }
        if (name == "Fatal" || name == "FatalException")
        {
            return isFatalEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }   
    
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
        if (name == "get_IsInfoEnabled")
        {
            return isInfoEnabledMethod;
        }
        if (name == "get_IsWarnEnabled")
        {
            return isWarnEnabledMethod;
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