using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    public OpCode GetLevelForLog(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name is "Debug" or "DebugException")
        {
            return OpCodes.Ldc_I4_2;
        }
        if (name is "Info" or "InfoException")
        {
            return OpCodes.Ldc_I4_3;
        }
        if (name is "Warn" or "WarnException")
        {
            return OpCodes.Ldc_I4_4;
        }
        if (name is "Error" or "ErrorException")
        {
            return OpCodes.Ldc_I4_5;
        }
        if (name is "Fatal" or "FatalException")
        {
            return OpCodes.Ldc_I4_6;
        }
        throw new("Invalid method name");
    }

    public OpCode GetLevel(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "get_IsDebugEnabled")
        {
            return OpCodes.Ldc_I4_2;
        }
        if (name == "get_IsInfoEnabled")
        {
            return OpCodes.Ldc_I4_3;
        }
        if (name == "get_IsWarnEnabled")
        {
            return OpCodes.Ldc_I4_4;
        }
        if (name == "get_IsErrorEnabled")
        {
            return OpCodes.Ldc_I4_5;
        }
        if (name == "get_IsFatalEnabled")
        {
            return OpCodes.Ldc_I4_6;
        }
        throw new("Invalid method name");
    }

    public MethodReference GetNormalOperandParams(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Debug")
        {
            return DebugMethodParams;
        }
        if (name == "Info")
        {
            return InfoMethodParams;
        }
        if (name == "Warn")
        {
            return WarnMethodParams;
        }
        if (name == "Error")
        {
            return ErrorMethodParams;
        }
        if (name == "Fatal")
        {
            return FatalMethodParams;
        }
        throw new("Invalid method name");
    }

    public MethodReference GetNormalOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "Debug")
        {
            return DebugMethod;
        }
        if (name == "Info")
        {
            return InfoMethod;
        }
        if (name == "Warn")
        {
            return WarnMethod;
        }
        if (name == "Error")
        {
            return ErrorMethod;
        }
        if (name == "Fatal")
        {
            return FatalMethod;
        }
        throw new("Invalid method name");
    }

    public MethodReference GetExceptionOperand(MethodReference methodReference)
    {
        var name = methodReference.Name;
        if (name == "DebugException")
        {
            return DebugExceptionMethod;
        }
        if (name == "InfoException")
        {
            return InfoExceptionMethod;
        }
        if (name == "WarnException")
        {
            return WarnExceptionMethod;
        }
        if (name == "ErrorException")
        {
            return ErrorExceptionMethod;
        }
        if (name == "FatalException")
        {
            return FatalExceptionMethod;
        }
        throw new("Invalid method name");
    }
}