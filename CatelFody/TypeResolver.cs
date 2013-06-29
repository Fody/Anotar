using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = CatelReference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerGenericDefinition = logManagerType.Methods.First(x => x.Name == "GetCurrentClassLogger");
		buildLoggerGenericMethod = ModuleDefinition.Import(getLoggerGenericDefinition);
		var loggerTypeDefinition = CatelReference.MainModule.Types.First(x => x.Name == "ILog");
		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
        var logInfoDefinition = logManagerType.NestedTypes.First(x => x.Name == "LogInfo");
        LogInfoType = ModuleDefinition.Import(logInfoDefinition);
        var logEventDefinition = CatelReference.MainModule.Types.First(x => x.Name == "LogEvent");
        DebugLogEvent = (int)logEventDefinition.Fields.First(x => x.Name == "Debug").Constant;
        ErrorLogEvent = (int)logEventDefinition.Fields.First(x => x.Name == "Error").Constant;
        InfoLogEvent = (int)logEventDefinition.Fields.First(x => x.Name == "Info").Constant;
        WarningLogEvent = (int)logEventDefinition.Fields.First(x => x.Name == "Warning").Constant;

        WriteMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("WriteWithData", "String", "Object", "LogEvent"));
        WriteExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("WriteWithData", "Exception", "String", "Object", "LogEvent"));

        isDebugEnabledMethod = ModuleDefinition.Import(logInfoDefinition.FindMethod("get_IsDebugEnabled"));
        isErrorEnabledMethod = ModuleDefinition.Import(logInfoDefinition.FindMethod("get_IsErrorEnabled"));
        isWarningEnabledMethod = ModuleDefinition.Import(logInfoDefinition.FindMethod("get_IsWarningEnabled"));
        isInfoEnabledMethod = ModuleDefinition.Import(logInfoDefinition.FindMethod("get_IsInfoEnabled"));
    }

    public int WarningLogEvent;
    public int InfoLogEvent;
    public int ErrorLogEvent;
    public int DebugLogEvent;

    public MethodReference WriteMethod;
	public MethodReference WriteExceptionMethod;
    public TypeReference LogInfoType;

	public TypeReference LoggerType;

	MethodReference buildLoggerGenericMethod; 
	public MethodReference isDebugEnabledMethod;
	public MethodReference isInfoEnabledMethod;
	public MethodReference isWarningEnabledMethod;
	public MethodReference isErrorEnabledMethod;

}