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

        forContextDefinition = ModuleDefinition.ImportReference(logManagerType.Methods.First(x => x.Name == "ForContext" && x.HasGenericParameters && x.IsMatch()));

        var loggerTypeDefinition = SerilogReference.MainModule.Types.First(x => x.Name == "ILogger");

        ForPropertyContextDefinition = ModuleDefinition.ImportReference(loggerTypeDefinition.Methods.First(x => x.Name == "ForContext" && !x.IsStatic && !x.HasGenericParameters && x.IsMatch("String", "Object", "Boolean")));


        IsEnabledMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("IsEnabled", "LogEventLevel"));

        debugMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]"));
        DebugExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]"));
        infoMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Information", "String", "Object[]"));
        InfoExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]"));
        warningMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]"));
        WarnExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]"));
        errorMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
        ErrorExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]"));
        fatalMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
        FatalExceptionMethod = ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]"));
        loggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
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