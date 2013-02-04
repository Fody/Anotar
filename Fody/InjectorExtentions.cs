using System;
using Mono.Cecil;

public static class InjectorExtentions
{
    public static MethodReference GetNormalOperand(this IInjector injector, MethodReference methodReference)
    {
        if (methodReference.Name == "Trace")
        {
            return injector.TraceMethod;
        }
        if (methodReference.Name == "Debug")
        {
            return injector.DebugMethod;
        }
        if (methodReference.Name == "Info")
        {
            return injector.InfoMethod;
        }
        if (methodReference.Name == "Warn")
        {
            return injector.WarnMethod;
        }
        if (methodReference.Name == "Error")
        {
            return injector.ErrorMethod;
        }
        throw new Exception("Invalid method name");
    }

    public static MethodReference GetExceptionOperand(this IInjector injector, MethodReference methodReference)
    {
        if (methodReference.Name == "TraceException")
        {
            return injector.TraceExceptionMethod;
        }
        if (methodReference.Name == "DebugException")
        {
            return injector.DebugExceptionMethod;
        }
        if (methodReference.Name == "InfoException")
        {
            return injector.InfoExceptionMethod;
        }
        if (methodReference.Name == "WarnException")
        {
            return injector.WarnExceptionMethod;
        }
        if (methodReference.Name == "ErrorException")
        {
            return injector.ErrorExceptionMethod;
        }
        throw new Exception("Invalid method name");
    }
}