using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = FindTypeDefinition("Catel.Logging.LogManager");
        var getLoggerMethod = logManagerType.FindMethod("GetLogger", "Type");
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerMethod);
        var loggerTypeDefinition = FindTypeDefinition("Catel.Logging.ILog");
        var logExtensionsDefinition = FindTypeDefinition("Catel.Logging.LogExtensions");
        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
        var logEventDefinition = FindTypeDefinition("Catel.Logging.LogEvent");
        DebugLogEvent = (int) logEventDefinition.Fields.First(_ => _.Name == "Debug").Constant;
        ErrorLogEvent = (int) logEventDefinition.Fields.First(_ => _.Name == "Error").Constant;
        InfoLogEvent = (int) logEventDefinition.Fields.First(_ => _.Name == "Info").Constant;
        WarningLogEvent = (int) logEventDefinition.Fields.First(_ => _.Name == "Warning").Constant;

        WriteMethod = ModuleDefinition.ImportReference(
            loggerTypeDefinition.FindMethod("WriteWithData", "String", "Object", "LogEvent"));
        var writeExceptionMethodRef = logExtensionsDefinition.FindMethod("WriteWithData", "ILog", "Exception", "String", "Object", "LogEvent");

        WriteExceptionMethod = ModuleDefinition.ImportReference(writeExceptionMethodRef);

        var logInfoDefinition = logManagerType.NestedTypes.First(_ => _.Name == "LogInfo");
        IsDebugEnabledMethod = ModuleDefinition.ImportReference(logInfoDefinition.FindMethod("get_IsDebugEnabled"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(logInfoDefinition.FindMethod("get_IsErrorEnabled"));
        IsWarningEnabledMethod = ModuleDefinition.ImportReference(logInfoDefinition.FindMethod("get_IsWarningEnabled"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(logInfoDefinition.FindMethod("get_IsInfoEnabled"));
    }

    public int WarningLogEvent;
    public int InfoLogEvent;
    public int ErrorLogEvent;
    public int DebugLogEvent;

    public MethodReference WriteMethod;
    public MethodReference WriteExceptionMethod;

    public TypeReference LoggerType;

    MethodReference constructLoggerMethod;
    public MethodReference IsDebugEnabledMethod;
    public MethodReference IsInfoEnabledMethod;
    public MethodReference IsWarningEnabledMethod;
    public MethodReference IsErrorEnabledMethod;
}