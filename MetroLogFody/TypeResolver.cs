using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerFactoryType = MetroLogReference.MainModule.Types.First(x => x.Name == "LogManagerFactory");
        var getDefaultLogManagerDefinition =
            logManagerFactoryType.Methods.First(x => x.Name == "get_DefaultLogManager");
        GetDefaultLogManager = ModuleDefinition.ImportReference(getDefaultLogManagerDefinition);

        var logManagerType = MetroLogReference.MainModule.Types.First(x => x.Name == "ILogManager");
        var getLoggerDefinition =
            logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String", "LoggingConfiguration"));
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);
        var loggerType = MetroLogReference.MainModule.Types.First(x => x.Name == "ILogger");

        TraceMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Trace", "String", "Object[]"));
        IsTraceEnabledMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Trace", "String", "Exception"));

        DebugMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Debug", "String", "Object[]"));
        IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Debug", "String", "Exception"));

        InfoMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Info", "String", "Object[]"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Info", "String", "Exception"));

        WarnMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Warn", "String", "Object[]"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Warn", "String", "Exception"));

        ErrorMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Error", "String", "Object[]"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Error", "String", "Exception"));

        FatalMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Fatal", "String", "Object[]"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.ImportReference(loggerType.FindMethod("Fatal", "String", "Exception"));

        LoggerType = ModuleDefinition.ImportReference(loggerType);
    }

    public MethodReference TraceMethod;
    public MethodReference TraceExceptionMethod;

    public MethodReference DebugMethod;
    public MethodReference DebugExceptionMethod;
    public MethodReference InfoMethod;
    public MethodReference InfoExceptionMethod;
    public MethodReference WarnMethod;
    public MethodReference WarnExceptionMethod;
    public MethodReference ErrorMethod;
    public MethodReference ErrorExceptionMethod;
    public MethodReference FatalMethod;
    public MethodReference FatalExceptionMethod;

    public TypeReference LoggerType;

    MethodReference constructLoggerMethod;


    public MethodReference GetDefaultLogManager;
    public MethodReference IsTraceEnabledMethod;
    public MethodReference IsInfoEnabledMethod;
    public MethodReference IsWarnEnabledMethod;
    public MethodReference IsErrorEnabledMethod;
    public MethodReference IsFatalEnabledMethod;
    public MethodReference IsDebugEnabledMethod;
}