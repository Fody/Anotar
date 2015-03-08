using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    public OpCode GetLevel(MethodReference methodReference)
    {
        if (methodReference.Name == "get_IsDebugEnabled")
        {
            return OpCodes.Ldc_I4_2;
        }
        if (methodReference.Name == "get_IsInfoEnabled")
        {
            return OpCodes.Ldc_I4_3;
        }
        if (methodReference.Name == "get_IsWarnEnabled")
        {
            return OpCodes.Ldc_I4_4;
        }
        if (methodReference.Name == "get_IsErrorEnabled")
        {
            return OpCodes.Ldc_I4_5;
        }
        if (methodReference.Name == "get_IsFatalEnabled")
        {
            return OpCodes.Ldc_I4_6;
        }
        throw new Exception("Invalid method name");
    }
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