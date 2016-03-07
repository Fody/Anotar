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
        DebugMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "Object"));
		IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Exception", "Object[]"));

        TraceMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("TraceFormat", "String", "Object[]"));
        TraceMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "Object"));
        IsTraceEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("TraceFormat", "String", "Exception", "Object[]"));

        InfoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Object[]"));
        InfoMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "Object"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Exception", "Object[]"));

        WarnMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Object[]"));
        WarnMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "Object"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Exception", "Object[]"));

        ErrorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Object[]"));
        ErrorMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "Object"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Exception", "Object[]"));

        FatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Object[]"));
        FatalMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "Object"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Exception", "Object[]"));

        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
    }


	public MethodReference DebugMethod;
	public MethodReference DebugMethodSimple;
	public MethodReference DebugExceptionMethod;
	public MethodReference InfoMethod;
	public MethodReference InfoMethodSimple;
	public MethodReference InfoExceptionMethod;
	public MethodReference WarnMethod;
	public MethodReference WarnMethodSimple;
	public MethodReference WarnExceptionMethod;
	public MethodReference ErrorMethod;
	public MethodReference ErrorMethodSimple;
	public MethodReference ErrorExceptionMethod;
	public MethodReference FatalMethod;
	public MethodReference FatalMethodSimple;
	public MethodReference FatalExceptionMethod;
	public MethodReference TraceMethod;
	public MethodReference TraceMethodSimple;
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