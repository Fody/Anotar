using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = NLogReference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerGenericDefinition = logManagerType.Methods.First(x => x.Name == "GetCurrentClassLogger");
		buildLoggerGenericMethod = ModuleDefinition.Import(getLoggerGenericDefinition);
		var loggerTypeDefinition = NLogReference.MainModule.Types.First(x => x.Name == "Logger");
		var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        buildLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);

        TraceMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Object[]"));
		isTraceEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
		TraceExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("TraceException", "String", "Exception"));
		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String","Object[]"));
		isDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("DebugException", "String", "Exception"));
        InfoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Object[]"));
		isInfoEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("InfoException", "String", "Exception"));
        WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Object[]"));
		isWarnEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("WarnException", "String", "Exception"));
        ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
		isErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("ErrorException", "String", "Exception"));
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

	public TypeReference LoggerType;

	MethodReference buildLoggerGenericMethod; MethodReference buildLoggerMethod;
	public MethodReference isTraceEnabledMethod;
	public MethodReference isDebugEnabledMethod;
	public MethodReference isInfoEnabledMethod;
	public MethodReference isWarnEnabledMethod;
	public MethodReference isErrorEnabledMethod;

}