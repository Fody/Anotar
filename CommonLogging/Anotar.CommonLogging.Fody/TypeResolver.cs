using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = FindTypeDefinition("Common.Logging.LogManager");

        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);

        var loggerTypeDefinition = FindTypeDefinition("Common.Logging.ILog");

        DebugFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Object[]"));
        DebugMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "Object"));
        IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("DebugFormat", "String", "Exception", "Object[]"));

        TraceFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("TraceFormat", "String", "Object[]"));
        TraceMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "Object"));
        IsTraceEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("TraceFormat", "String", "Exception", "Object[]"));

        InfoFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Object[]"));
        InfoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "Object"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("InfoFormat", "String", "Exception", "Object[]"));

        WarnFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Object[]"));
        WarnMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "Object"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("WarnFormat", "String", "Exception", "Object[]"));

        ErrorFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Object[]"));
        ErrorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "Object"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Exception", "Object[]"));

        FatalFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Object[]"));
        FatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "Object"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("FatalFormat", "String", "Exception", "Object[]"));

        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
    }


    public MethodReference DebugFormatMethod;
    public MethodReference DebugMethod;
    public MethodReference DebugExceptionMethod;
    public MethodReference InfoFormatMethod;
    public MethodReference InfoMethod;
    public MethodReference InfoExceptionMethod;
    public MethodReference WarnFormatMethod;
    public MethodReference WarnMethod;
    public MethodReference WarnExceptionMethod;
    public MethodReference ErrorFormatMethod;
    public MethodReference ErrorMethod;
    public MethodReference ErrorExceptionMethod;
    public MethodReference FatalFormatMethod;
    public MethodReference FatalMethod;
    public MethodReference FatalExceptionMethod;
    public MethodReference TraceFormatMethod;
    public MethodReference TraceMethod;
    public MethodReference TraceExceptionMethod;
    public TypeReference LoggerType;
    MethodReference constructLoggerMethod;
    public MethodReference IsErrorEnabledMethod;
    public MethodReference IsFatalEnabledMethod;
    public MethodReference IsDebugEnabledMethod;
    public MethodReference IsInfoEnabledMethod;
    public MethodReference IsWarnEnabledMethod;
    public MethodReference IsTraceEnabledMethod;
}