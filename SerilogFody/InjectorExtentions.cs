using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public MethodReference GetNormalOperand(MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return debugMethod;
        }
        if (methodReference.Name == "Information")
        {
            return infoMethod;
        }
        if (methodReference.Name == "Warning")
        {
            return warnMethod;
        }
        if (methodReference.Name == "Error")
        {
            return errorMethod;
        }
        if (methodReference.Name == "Fatal")
        {
			return fatalMethod;
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