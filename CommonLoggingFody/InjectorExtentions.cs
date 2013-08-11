using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetNormalOperand(MethodReference methodReference)
    {
        if (methodReference.Name == "Trace")
        {
            return TraceMethod;
        }
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
        if (methodReference.Name == "TraceException")
        {
            return TraceExceptionMethod;
        }
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