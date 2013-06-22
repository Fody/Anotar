using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var loggerTypeDefinition = GetLoggerMethod.ReturnType.Resolve();
        
		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String","Object[]"));
		isDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]"));
        InformationMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "String", "Object[]"));
        isInformationEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInformationEnabled"));
        InformationExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]"));
        WarningMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]"));
        isWarningEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarningEnabled"));
        WarningExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]"));
        ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
		isErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]"));
        FatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
        isFatalEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]"));
		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
    }

	public MethodReference DebugMethod;
	public MethodReference DebugExceptionMethod;
    public MethodReference InformationMethod;
    public MethodReference InformationExceptionMethod;
    public MethodReference WarningMethod;
	public MethodReference WarningExceptionMethod;
	public MethodReference ErrorMethod;
    public MethodReference ErrorExceptionMethod;
	public MethodReference FatalMethod;
	public MethodReference FatalExceptionMethod;

	public TypeReference LoggerType;

	public MethodReference isDebugEnabledMethod;
    public MethodReference isInformationEnabledMethod;
    public MethodReference isWarningEnabledMethod;
	public MethodReference isErrorEnabledMethod;
	public MethodReference isFatalEnabledMethod;

}