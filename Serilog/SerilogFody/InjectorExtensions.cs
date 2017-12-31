using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public int GetLevelForMethodName(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Verbose")
        {
            return VerboseLevel;
        }
        if (name == "Debug")
        {
            return DebugLevel;
        }
        if (name == "Information")
        {
            return InformationLevel;
        }
        if (name == "Warning")
        {
            return WarningLevel;
        }
        if (name == "Error")
        {
            return ErrorLevel;
        }
        if (name == "Fatal")
        {
            return FatalLevel;
        }
        throw new Exception("Invalid method name");
    }

    public int GetLevelForIsEnabled(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "get_IsVerboseEnabled")
        {
            return VerboseLevel;
        }
        if (name == "get_IsDebugEnabled")
        {
            return DebugLevel;
        }
        if (name == "get_IsInformationEnabled")
        {
            return InformationLevel;
        }
        if (name == "get_IsWarningEnabled")
        {
            return WarningLevel;
        }
        if (name == "get_IsErrorEnabled")
        {
            return ErrorLevel;
        }
        if (name == "get_IsFatalEnabled")
        {
            return FatalLevel;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetNormalOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Verbose")
        {
            return verboseMethod;
        }
        if (name == "Debug")
        {
            return debugMethod;
        }
        if (name == "Information")
        {
            return infoMethod;
        }
        if (name == "Warning")
        {
            return warningMethod;
        }
        if (name == "Error")
        {
            return errorMethod;
        }
        if (name == "Fatal")
        {
            return fatalMethod;
        }
        throw new Exception("Invalid method name");
    }

    public MethodReference GetExceptionOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Verbose")
        {
            return VerboseExceptionMethod;
        }
        if (name == "Debug")
        {
            return DebugExceptionMethod;
        }
        if (name == "Information")
        {
            return InfoExceptionMethod;
        }
        if (name == "Warning")
        {
            return WarnExceptionMethod;
        }
        if (name == "Error")
        {
            return ErrorExceptionMethod;
        }
        if (name == "Fatal")
        {
            return FatalExceptionMethod;
        }
        throw new Exception("Invalid method name");
    }
}