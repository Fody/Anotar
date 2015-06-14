using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = NLogReference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerGenericDefinition = logManagerType.Methods.First(x => x.Name == "GetCurrentClassLogger");
		constructLoggerGenericMethod = ModuleDefinition.Import(getLoggerGenericDefinition);
		var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        constructLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);

        var loggerTypeDefinition = NLogReference.MainModule.Types.First(x => x.Name == "Logger");
        TraceMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Object[]"));
		IsTraceEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
		TraceExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("TraceException", "String", "Exception"));
		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String","Object[]"));
		IsDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("DebugException", "String", "Exception"));
        InfoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Object[]"));
		IsInfoEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("InfoException", "String", "Exception"));
        WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Object[]"));
		IsWarnEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("WarnException", "String", "Exception"));
        ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
		IsErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("ErrorException", "String", "Exception"));
        FatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
        IsFatalEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("FatalException", "String", "Exception"));
		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
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