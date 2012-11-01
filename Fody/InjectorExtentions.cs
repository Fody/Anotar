using System;
using Mono.Cecil;

public static class InjectorExtentions
{
    public static MethodReference GetNormalOperand(this IInjector injector , MethodReference methodReference)
    {
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
    public static MethodReference GetExceptionOperand(this IInjector injector , MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
                return injector.DebugExceptionMethod;
        }
        if (methodReference.Name == "Info")
        {
                return injector.InfoExceptionMethod;
        }
        if (methodReference.Name == "Warn")
        {
                return injector.WarnExceptionMethod;
        }
        if (methodReference.Name == "Error")
        {
                return injector.ErrorExceptionMethod;
        }
        throw new Exception("Invalid method name");
    }
}