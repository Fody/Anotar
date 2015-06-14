using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = NServiceBusReference.MainModule.Types.First(x => x.Name == "LogManager");
		var getLoggerDefinition = logManagerType.FindMethod("GetLogger","String");
        constructLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);

        var loggerTypeDefinition = NServiceBusReference.MainModule.Types.First(x => x.Name == "ILog");
		DebugFormatMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("DebugFormat", "String","Object[]"));
		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String"));
		IsDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));
        InfoFormatMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("InfoFormat", "String", "Object[]"));
        InfoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String"));
		IsInfoEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));
        WarnFormatMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("WarnFormat", "String", "Object[]"));
        WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String"));
		IsWarnEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));
        ErrorFormatMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("ErrorFormat", "String", "Object[]"));
        ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String"));
		IsErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));
        FatalFormatMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("FatalFormat", "String", "Object[]"));
        FatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String"));
        IsFatalEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Exception"));
		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
    }

	public MethodReference DebugFormatMethod;
	public MethodReference DebugMethod;
	public MethodReference DebugExceptionMethod;
	public MethodReference InfoFormatMethod;
	public MethodReference InfoMethod;
	public MethodReference InfoExceptionMethod;
	public MethodReference WarnFormatMethod;
	public MethodReference WarnMethod;
	public MethodReference WarnExceptionMethod;
	public MethodReference ErrorFormatMethod;
	public MethodReference ErrorMethod;
    public MethodReference ErrorExceptionMethod;
	public MethodReference FatalFormatMethod;
	public MethodReference FatalMethod;
	public MethodReference FatalExceptionMethod;

	public TypeReference LoggerType;

    MethodReference constructLoggerMethod;
	public MethodReference IsDebugEnabledMethod;
	public MethodReference IsInfoEnabledMethod;
	public MethodReference IsWarnEnabledMethod;
	public MethodReference IsErrorEnabledMethod;
	public MethodReference IsFatalEnabledMethod;

}