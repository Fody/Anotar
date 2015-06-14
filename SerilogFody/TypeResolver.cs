using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
		var logManagerType = SerilogReference.MainModule.Types.First(x => x.Name == "Log");
		var logEventLevelType = SerilogReference.MainModule.Types.First(x => x.Name == "LogEventLevel");
	    DebugLevel = (int) logEventLevelType.Fields.First(x => x.Name == "Debug").Constant;
		ErrorLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Error").Constant;
	    InformationLevel = (int) logEventLevelType.Fields.First(x => x.Name == "Information").Constant;
	    WarningLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Warning").Constant;
		FatalLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Fatal").Constant;

		forContextDefinition = ModuleDefinition.Import(logManagerType.Methods.First(x => x.Name == "ForContext" && x.HasGenericParameters && x.IsMatch()));

        var loggerTypeDefinition = SerilogReference.MainModule.Types.First(x => x.Name == "ILogger");

        ForPropertyContextDefinition = ModuleDefinition.Import(loggerTypeDefinition.Methods.First(x => x.Name == "ForContext" && !x.IsStatic && !x.HasGenericParameters && x.IsMatch("String", "Object", "Boolean")));


		IsEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("IsEnabled", "LogEventLevel"));

		debugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]"));
		infoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "String", "Object[]"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]"));
		warningMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]"));
		errorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]"));
		fatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
		FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]"));
		loggerType = ModuleDefinition.Import(loggerTypeDefinition);
    }

    MethodReference debugMethod;
    MethodReference infoMethod;
    MethodReference warningMethod;
    MethodReference errorMethod; 
    MethodReference fatalMethod;
    TypeReference loggerType;

	public MethodReference DebugExceptionMethod;
	public MethodReference InfoExceptionMethod;
	public MethodReference WarnExceptionMethod;
	public MethodReference ErrorExceptionMethod;
	public MethodReference FatalExceptionMethod;


	MethodReference forContextDefinition;
	public MethodReference IsEnabledMethod;
	public int DebugLevel;
	public int FatalLevel;
	public int WarningLevel;
	public int ErrorLevel;
	public int InformationLevel;
    public MethodReference ForPropertyContextDefinition;
}