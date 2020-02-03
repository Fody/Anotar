using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = FindTypeDefinition("NServiceBus.Logging.LogManager");
        var getLoggerDefinition = logManagerType.FindMethod("GetLogger", "String");
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);

        var loggerTypeDefinition = FindTypeDefinition("NServiceBus.Logging.ILog");

        DebugFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Object[]"));
        DebugMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String"));
        IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));

        InfoFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Object[]"));
        InfoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "String"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));

        WarnFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Object[]"));
        WarnMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "String"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));

        ErrorFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Object[]"));
        ErrorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));

        FatalFormatMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Object[]"));
        FatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod =
            ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String", "Exception"));

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

    public TypeReference LoggerType;

    MethodReference constructLoggerMethod;
    public MethodReference IsDebugEnabledMethod;
    public MethodReference IsInfoEnabledMethod;
    public MethodReference IsWarnEnabledMethod;
    public MethodReference IsErrorEnabledMethod;
    public MethodReference IsFatalEnabledMethod;
}