using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = FindTypeDefinition("NLog.LogManager");
        var getLoggerGenericDefinition = logManagerType.Methods.First(x => x.Name == "GetCurrentClassLogger");
        constructLoggerGenericMethod = ModuleDefinition.ImportReference(getLoggerGenericDefinition);
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);

        var loggerTypeDefinition = FindTypeDefinition("NLog.Logger");

        TraceFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "String", "Object[]"));
        TraceMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "String"));
        IsTraceEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("TraceException", "String", "Exception"));

        DebugFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]"));
        DebugMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String"));
        IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugException", "String", "Exception"));

        InfoFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "String", "Object[]"));
        InfoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "String"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoException", "String", "Exception"));

        WarnFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "String", "Object[]"));
        WarnMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "String"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnException", "String", "Exception"));

        ErrorFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
        ErrorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorException", "String", "Exception"));

        FatalFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
        FatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalException", "String", "Exception"));

        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
    }

    public MethodReference TraceMethod;
    public MethodReference TraceFormatMethod;
    public MethodReference TraceExceptionMethod;
    public MethodReference DebugMethod;
    public MethodReference DebugFormatMethod;
    public MethodReference DebugExceptionMethod;
    public MethodReference InfoMethod;
    public MethodReference InfoFormatMethod;
    public MethodReference InfoExceptionMethod;
    public MethodReference WarnMethod;
    public MethodReference WarnFormatMethod;
    public MethodReference WarnExceptionMethod;
    public MethodReference ErrorMethod;
    public MethodReference ErrorFormatMethod;
    public MethodReference ErrorExceptionMethod;
    public MethodReference FatalMethod;
    public MethodReference FatalFormatMethod;
    public MethodReference FatalExceptionMethod;

    public TypeReference LoggerType;

    MethodReference constructLoggerGenericMethod;
    MethodReference constructLoggerMethod;
    public MethodReference IsTraceEnabledMethod;
    public MethodReference IsDebugEnabledMethod;
    public MethodReference IsInfoEnabledMethod;
    public MethodReference IsWarnEnabledMethod;
    public MethodReference IsErrorEnabledMethod;
    public MethodReference IsFatalEnabledMethod;
}