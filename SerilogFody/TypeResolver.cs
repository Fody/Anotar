using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
		var logManagerType = SerilogReference.MainModule.Types.First(x => x.Name == "Log");
		var logEventLevelType = SerilogReference.MainModule.Types.First(x => x.Name == "LogEventLevel");
	    debugLevel = (int) logEventLevelType.Fields.First(x => x.Name == "Debug").Constant;
		errorLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Error").Constant;
	    infoLevel = (int) logEventLevelType.Fields.First(x => x.Name == "Information").Constant;
	    warningLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Warning").Constant;
		fatalLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Fatal").Constant;

		forContextDefinition = ModuleDefinition.Import(logManagerType.Methods.First(x => x.Name == "ForContext" && x.HasGenericParameters && x.IsMatch()));

		var loggerTypeDefinition = SerilogReference.MainModule.Types.First(x => x.Name == "ILogger");

		isEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("IsEnabled", "LogEventLevel"));

		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]"));
		InfoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "String", "Object[]"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]"));
		WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]"));
		ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]"));
		FatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
		FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]"));
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

	public TypeReference LoggerType;

	MethodReference forContextDefinition;
	public MethodReference isEnabledMethod;
	public int debugLevel;
	public int fatalLevel;
	public int warningLevel;
	public int errorLevel;
	public int infoLevel;
}