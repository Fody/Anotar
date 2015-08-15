using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = NLogReference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerGenericDefinition = logManagerType.Methods.First(x => x.Name == "GetCurrentClassLogger");
        constructLoggerGenericMethod = ModuleDefinition.ImportReference(getLoggerGenericDefinition);
		var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);

        var loggerTypeDefinition = NLogReference.MainModule.Types.First(x => x.Name == "Logger");
        TraceMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "String", "Object[]"));
        IsTraceEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("TraceException", "String", "Exception"));
        DebugMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]"));
        IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugException", "String", "Exception"));
        InfoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "String", "Object[]"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoException", "String", "Exception"));
        WarnMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "String", "Object[]"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnException", "String", "Exception"));
        ErrorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorException", "String", "Exception"));
        FatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalException", "String", "Exception"));
        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
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

	MethodReference constructLoggerGenericMethod; 
    MethodReference constructLoggerMethod;
	public MethodReference IsTraceEnabledMethod;
	public MethodReference IsDebugEnabledMethod;
	public MethodReference IsInfoEnabledMethod;
	public MethodReference IsWarnEnabledMethod;
	public MethodReference IsErrorEnabledMethod;
	public MethodReference IsFatalEnabledMethod;

}