using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public int GetLevelForMethodName(MethodReference methodReference)
    {
        if (methodReference.Name == "Debug")
        {
            return DebugLevel;
        }
        if (methodReference.Name == "Information")
        {
            return InformationLevel;
        }
        if (methodReference.Name == "Warning")
        {
            return WarningLevel;
        }
        if (methodReference.Name == "Error")
        {
            return ErrorLevel;
        }
        if (methodReference.Name == "Fatal")
        {
            return FatalLevel;
        }
        throw new Exception("Invalid method name");
    }
    public int GetLevelForIsEnabled(MethodReference methodReference)
    {
        if (methodReference.Name == "get_IsDebugEnabled")
        {
            return DebugLevel;
        }
        if (methodReference.Name == "get_IsInformationEnabled")
        {
            return InformationLevel;
        }
        if (methodReference.Name == "get_IsWarningEnabled")
        {
            return WarningLevel;
        }
        if (methodReference.Name == "get_IsErrorEnabled")
        {
            return ErrorLevel;
        }
        if (methodReference.Name == "get_IsFatalEnabled")
        {
            return FatalLevel;
        }
        throw new Exception("Invalid method name");
    }
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
            return warningMethod;
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