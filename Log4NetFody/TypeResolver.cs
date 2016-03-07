using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public void Init()
    {

		var logManagerType = Log4NetReference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        constructLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);
		var loggerTypeDefinition = Log4NetReference.MainModule.Types.First(x => x.Name == "ILog");

        DebugMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("DebugFormat", "String", "Object[]"));
        DebugMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "Object"));
        IsDebugEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "Object", "Exception"));

        InfoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Object[]"));
        InfoMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "Object"));
        IsInfoEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Info", "Object", "Exception"));

        WarnMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Object[]"));
        WarnMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "Object"));
        IsWarnEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warn", "Object", "Exception"));

        ErrorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Object[]"));
        ErrorMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "Object"));
        IsErrorEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "Object", "Exception"));

        FatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Object[]"));
        FatalMethodSimple = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "Object"));
        IsFatalEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "Object", "Exception"));
        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
    }


	public MethodReference DebugMethodSimple;
	public MethodReference DebugMethod;
	public MethodReference DebugExceptionMethod;
	public MethodReference InfoMethodSimple;
	public MethodReference InfoMethod;
	public MethodReference InfoExceptionMethod;
	public MethodReference WarnMethodSimple;
	public MethodReference WarnMethod;
	public MethodReference WarnExceptionMethod;
	public MethodReference ErrorMethodSimple;
	public MethodReference ErrorMethod;
	public MethodReference ErrorExceptionMethod;
	public MethodReference FatalMethodSimple;
	public MethodReference FatalMethod;
	public MethodReference FatalExceptionMethod;
	public TypeReference LoggerType;
    MethodReference constructLoggerMethod;
	public MethodReference IsErrorEnabledMethod;
	public MethodReference IsFatalEnabledMethod;
	public MethodReference IsDebugEnabledMethod;
	public MethodReference IsInfoEnabledMethod;
	public MethodReference IsWarnEnabledMethod;
}