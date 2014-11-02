using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public void Init()
    {

		var logManagerType = CommonLoggingReference.Value.MainModule.Types.First(x => x.Name == "LogManager");
        
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        buildLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);

        var loggerTypeDefinition = CommonLoggingReference.Value.MainModule.Types.First(x => x.Name == "ILog");

        DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Object[]"));
		isDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Exception", "Object[]"));

        TraceMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("TraceFormat", "String", "Object[]"));
        isTraceEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("TraceFormat", "String", "Exception", "Object[]"));

        InfoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Object[]"));
		isInfoEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Exception", "Object[]"));

        WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Object[]"));
		isWarnEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Exception", "Object[]"));

        ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Object[]"));
		isErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Exception", "Object[]"));

        FatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Object[]"));
		isFatalEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Exception", "Object[]"));

		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
    }


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
	public MethodReference TraceMethod;
    public MethodReference TraceExceptionMethod;
	public TypeReference LoggerType;
    MethodReference buildLoggerMethod;
	public MethodReference isErrorEnabledMethod;
	public MethodReference isFatalEnabledMethod;
	public MethodReference isDebugEnabledMethod;
	public MethodReference isInfoEnabledMethod;
	public MethodReference isWarnEnabledMethod;
	public MethodReference isTraceEnabledMethod;
}