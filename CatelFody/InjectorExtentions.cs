using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    //public MethodReference GetNormalOperand(MethodReference methodReference)
    //{
    //    if (methodReference.Name == "Debug")
    //    {
    //        return DebugMethod;
    //    }
    //    if (methodReference.Name == "Info")
    //    {
    //        return InfoMethod;
    //    }
    //    if (methodReference.Name == "Warning")
    //    {
    //        return WarningMethod;
    //    }
    //    if (methodReference.Name == "Error")
    //    {
    //        return ErrorMethod;
    //    }
    //    throw new Exception("Invalid method name");
    //}
    public int GetLogEvent(MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return DebugLogEvent;
        }
        if (methodReference.Name == "Info")
        {
            return InfoLogEvent;
        }
        if (methodReference.Name == "Warning")
        {
            return WarningLogEvent;
        }
        if (methodReference.Name == "Error")
        {
            return ErrorLogEvent;
        }
        throw new Exception("Invalid method name");
    }

    //public MethodReference GetExceptionOperand(MethodReference methodReference)
    //{
    //    if (methodReference.Name == "DebugException")
    //    {
    //        return DebugExceptionMethod;
    //    }
    //    if (methodReference.Name == "InfoException")
    //    {
    //        return InfoExceptionMethod;
    //    }
    //    if (methodReference.Name == "WarningException")
    //    {
    //        return WarningExceptionMethod;
    //    }
    //    if (methodReference.Name == "ErrorException")
    //    {
    //        return ErrorExceptionMethod;
    //    }
    //    throw new Exception("Invalid method name");
    //}
}