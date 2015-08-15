using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public void Init()
    {

		var logManagerType = CommonLoggingReference.MainModule.Types.First(x => x.Name == "LogManager");
        
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);

        var loggerTypeDefinition = CommonLoggingCoreReference.MainModule.Types.First(x => x.Name == "ILog");

        DebugMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Object[]"));
		IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Exception", "Object[]"));

        TraceMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("TraceFormat", "String", "Object[]"));
        IsTraceEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("TraceFormat", "String", "Exception", "Object[]"));

        InfoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Object[]"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Exception", "Object[]"));

        WarnMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Object[]"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Exception", "Object[]"));

        ErrorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Object[]"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Exception", "Object[]"));

        FatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Object[]"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Exception", "Object[]"));

        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
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
    MethodReference constructLoggerMethod;
	public MethodReference IsErrorEnabledMethod;
	public MethodReference IsFatalEnabledMethod;
	public MethodReference IsDebugEnabledMethod;
	public MethodReference IsInfoEnabledMethod;
	public MethodReference IsWarnEnabledMethod;
	public MethodReference IsTraceEnabledMethod;
}