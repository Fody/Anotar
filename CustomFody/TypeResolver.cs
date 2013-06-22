using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var loggerTypeDefinition = GetLoggerMethod.ReturnType.Resolve();
        
        TraceMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Object[]"));
		isTraceEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "Exception", "String", "Object[]"));
		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String","Object[]"));
		isDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]"));
        InformationMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "String", "Object[]"));
        isInformationEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInformationEnabled"));
        InformationExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]"));
        WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Object[]"));
		isWarnEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "Exception", "String", "Object[]"));
        ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
		isErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]"));
        FatalMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]"));
        isFatalEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsFatalEnabled"));
        FatalExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]"));
		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
    }

	public MethodReference TraceMethod;
	public MethodReference TraceExceptionMethod;
	public MethodReference DebugMethod;
	public MethodReference DebugExceptionMethod;
    public MethodReference InformationMethod;
    public MethodReference InformationExceptionMethod;
	public MethodReference WarnMethod;
	public MethodReference WarnExceptionMethod;
	public MethodReference ErrorMethod;
    public MethodReference ErrorExceptionMethod;
	public MethodReference FatalMethod;
	public MethodReference FatalExceptionMethod;

	public TypeReference LoggerType;

	public MethodReference isTraceEnabledMethod;
	public MethodReference isDebugEnabledMethod;
    public MethodReference isInformationEnabledMethod;
	public MethodReference isWarnEnabledMethod;
	public MethodReference isErrorEnabledMethod;
	public MethodReference isFatalEnabledMethod;

}