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
        if (methodReference.Name == "Information")
        {
            return InformationMethod;
        }
        if (methodReference.Name == "Warning")
        {
            return WarningMethod;
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
        if (methodReference.Name == "Trace")
        {
            return TraceExceptionMethod;
        }
        if (methodReference.Name == "Debug")
        {
            return DebugExceptionMethod;
        }
        if (methodReference.Name == "Information")
        {
            return InformationExceptionMethod;
        }
        if (methodReference.Name == "Warning")
        {
            return WarningExceptionMethod;
        }
        if (methodReference.Name == "Error")
        {
            return ErrorExceptionMethod;
        }
        if (methodReference.Name == "Fatal")
        {
            return FatalExceptionMethod;
        }
        throw new Exception("Invalid method name");
    }
}