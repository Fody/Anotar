using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var loggerTypeDefinition = GetLoggerMethod.ReturnType.Resolve();
        
		DebugMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String","Object[]")));
		isDebugEnabledMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled")));
        DebugExceptionMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]")));
        InformationMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "String", "Object[]")));
        isInformationEnabledMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInformationEnabled")));
        InformationExceptionMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]")));
        WarningMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]")));
        isWarningEnabledMethod = new Lazy<MethodReference>(()=>ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarningEnabled")));
        WarningExceptionMethod =new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]")));
        ErrorMethod =new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]")));
		isErrorEnabledMethod =new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled")));
        ErrorExceptionMethod = new Lazy<MethodReference>(()=>ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]")));
        FatalMethod = new Lazy<MethodReference>(()=>ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]")));
        isFatalEnabledMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsFatalEnabled")));
        FatalExceptionMethod = new Lazy<MethodReference>(()=> ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]")));
		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
    }

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

    public Lazy<MethodReference> isDebugEnabledMethod;
    public Lazy<MethodReference> isInformationEnabledMethod;
    public Lazy<MethodReference> isWarningEnabledMethod;
    public Lazy<MethodReference> isErrorEnabledMethod;
    public Lazy<MethodReference> isFatalEnabledMethod;

}