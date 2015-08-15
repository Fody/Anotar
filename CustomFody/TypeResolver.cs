using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var loggerTypeDefinition = GetLoggerMethod.ReturnType.Resolve();

        TraceMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "String", "Object[]")));
        IsTraceEnabledMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsTraceEnabled")));
        TraceExceptionMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "Exception", "String", "Object[]")));
        DebugMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]")));
        IsDebugEnabledMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled")));
        DebugExceptionMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]")));
        InformationMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Information", "String", "Object[]")));
        IsInformationEnabledMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInformationEnabled")));
        InformationExceptionMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]")));
        WarningMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]")));
        IsWarningEnabledMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarningEnabled")));
        WarningExceptionMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]")));
        ErrorMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String", "Object[]")));
        IsErrorEnabledMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled")));
        ErrorExceptionMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]")));
        FatalMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]")));
        IsFatalEnabledMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled")));
        FatalExceptionMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]")));
        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
    }

	public Lazy<MethodReference> TraceMethod;
	public Lazy<MethodReference> TraceExceptionMethod;
	public Lazy<MethodReference> DebugMethod;
	public Lazy<MethodReference> DebugExceptionMethod;
    public Lazy<MethodReference> InformationMethod;
    public Lazy<MethodReference> InformationExceptionMethod;
    public Lazy<MethodReference> WarningMethod;
	public Lazy<MethodReference> WarningExceptionMethod;
	public Lazy<MethodReference> ErrorMethod;
    public Lazy<MethodReference> ErrorExceptionMethod;
	public Lazy<MethodReference> FatalMethod;
	public Lazy<MethodReference> FatalExceptionMethod;

	public TypeReference LoggerType;

    public Lazy<MethodReference> IsTraceEnabledMethod;
    public Lazy<MethodReference> IsDebugEnabledMethod;
    public Lazy<MethodReference> IsInformationEnabledMethod;
    public Lazy<MethodReference> IsWarningEnabledMethod;
    public Lazy<MethodReference> IsErrorEnabledMethod;
    public Lazy<MethodReference> IsFatalEnabledMethod;

}