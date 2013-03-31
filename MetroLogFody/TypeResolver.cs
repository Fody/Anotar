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
		var loggerTypeDefinition = MetroLogReference.MainModule.Types.First(x => x.Name == "ILogger");

		TraceMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Exception"));
		isTraceEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
		TraceExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Exception"));
		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));
		isDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));
		InfoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));
		isInfoEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));
		WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));
		isWarnEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));
		ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));
		isErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));
		FatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Exception"));
		isFatalEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
		FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Exception"));
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

    MethodReference buildLoggerMethod;
    

   public MethodReference getDefaultLogManager;
   public MethodReference isTraceEnabledMethod;
   public MethodReference isInfoEnabledMethod;
   public MethodReference isWarnEnabledMethod;
   public MethodReference isErrorEnabledMethod;
   public MethodReference isFatalEnabledMethod;
   public MethodReference isDebugEnabledMethod;
}