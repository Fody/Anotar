using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public void Init()
    {
		var logManagerFactoryType = MetroLogReference.MainModule.Types.First(x => x.Name == "LogManagerFactory");
        var getDefaultLogManagerDefinition = logManagerFactoryType.Methods.First(x => x.Name == "get_DefaultLogManager");
        getDefaultLogManager = ModuleDefinition.Import(getDefaultLogManagerDefinition);

		var logManagerType = MetroLogReference.MainModule.Types.First(x => x.Name == "ILogManager");
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String", "LoggingConfiguration"));
		buildLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);
		var loggerType = MetroLogReference.MainModule.Types.First(x => x.Name == "ILogger");

		TraceMethod = ModuleDefinition.Import(loggerType.FindMethod("Trace", "String", "Object[]"));
		isTraceEnabledMethod = ModuleDefinition.Import(loggerType.FindMethod("get_IsTraceEnabled"));
		TraceExceptionMethod = ModuleDefinition.Import(loggerType.FindMethod("Trace", "String", "Exception"));
        DebugMethod = ModuleDefinition.Import(loggerType.FindMethod("Debug", "String", "Object[]"));
		isDebugEnabledMethod = ModuleDefinition.Import(loggerType.FindMethod("get_IsDebugEnabled"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerType.FindMethod("Debug", "String", "Exception"));
        InfoMethod = ModuleDefinition.Import(loggerType.FindMethod("Info", "String", "Object[]"));
		isInfoEnabledMethod = ModuleDefinition.Import(loggerType.FindMethod("get_IsInfoEnabled"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerType.FindMethod("Info", "String", "Exception"));
        WarnMethod = ModuleDefinition.Import(loggerType.FindMethod("Warn", "String", "Object[]"));
		isWarnEnabledMethod = ModuleDefinition.Import(loggerType.FindMethod("get_IsWarnEnabled"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerType.FindMethod("Warn", "String", "Exception"));
        ErrorMethod = ModuleDefinition.Import(loggerType.FindMethod("Error", "String", "Object[]"));
		isErrorEnabledMethod = ModuleDefinition.Import(loggerType.FindMethod("get_IsErrorEnabled"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerType.FindMethod("Error", "String", "Exception"));
        FatalMethod = ModuleDefinition.Import(loggerType.FindMethod("Fatal", "String", "Object[]"));
		isFatalEnabledMethod = ModuleDefinition.Import(loggerType.FindMethod("get_IsFatalEnabled"));
		FatalExceptionMethod = ModuleDefinition.Import(loggerType.FindMethod("Fatal", "String", "Exception"));
		LoggerType = ModuleDefinition.Import(loggerType);
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

    MethodReference buildLoggerMethod;
    

   public MethodReference getDefaultLogManager;
   public MethodReference isTraceEnabledMethod;
   public MethodReference isInfoEnabledMethod;
   public MethodReference isWarnEnabledMethod;
   public MethodReference isErrorEnabledMethod;
   public MethodReference isFatalEnabledMethod;
   public MethodReference isDebugEnabledMethod;
}