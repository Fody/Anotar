using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
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
        if (methodReference.Name == "get_IsWarnEnabled")
        {
            return isWarnEnabledMethod;
        }
        if (methodReference.Name == "get_IsErrorEnabled")
        {
            return isErrorEnabledMethod;
        }
        if (methodReference.Name == "get_IsFatalEnabled")
        {
            return isFatalEnabledMethod;
        }
        throw new Exception("Invalid method name");
    }
    public MethodReference GetNormalOperand(MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return DebugMethod;
        }
        if (methodReference.Name == "Info")
        {
            return InfoMethod;
        }
        if (methodReference.Name == "Warn")
        {
            return WarnMethod;
        }
        if (methodReference.Name == "Error")
        {
            return ErrorMethod;
        }
        if (methodReference.Name == "Fatal")
        {
            return FatalMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetExceptionOperand(MethodReference methodReference)
    {
        if (methodReference.Name == "DebugException")
        {
            return DebugExceptionMethod;
        }
        if (methodReference.Name == "InfoException")
        {
            return InfoExceptionMethod;
        }
        if (methodReference.Name == "WarnException")
        {
            return WarnExceptionMethod;
        }
        if (methodReference.Name == "ErrorException")
        {
            return ErrorExceptionMethod;
        }
        if (methodReference.Name == "FatalException")
        {
            return FatalExceptionMethod;
        }
        throw new Exception("Invalid method name");
    }
}