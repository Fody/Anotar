using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetNormalOperandParams(MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return DebugMethodParams;
        }
        if (methodReference.Name == "Info")
        {
            return InfoMethodParams;
        }
        if (methodReference.Name == "Warn")
        {
            return WarnMethodParams;
        }
        if (methodReference.Name == "Error")
        {
            return ErrorMethodParams;
        }
        if (methodReference.Name == "Fatal")
        {
            return FatalMethodParams;
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