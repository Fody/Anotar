using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetNormalOperand(MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return DebugMethod;
        }
        if (methodReference.Name == "Information")
        {
            return InfoMethod;
        }
        if (methodReference.Name == "Warning")
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

    public MethodReference GetExceptionOperand( MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return DebugExceptionMethod;
        }
        if (methodReference.Name == "Information")
        {
            return InfoExceptionMethod;
        }
        if (methodReference.Name == "Warning")
        {
            return WarnExceptionMethod;
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