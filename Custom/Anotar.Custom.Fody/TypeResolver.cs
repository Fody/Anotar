using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var loggerTypeDefinition = GetLoggerMethod.ReturnType.Resolve();

        TraceFormatMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "String", "Object[]")));
        TraceMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Trace", "String")));
        IsTraceEnabledMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsTraceEnabled")));
        TraceExceptionMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("Trace", "Exception", "String", "Object[]")));

        DebugFormatMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]")));
        DebugMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Debug", "String")));
        IsDebugEnabledMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsDebugEnabled")));
        DebugExceptionMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]")));

        InformationFormatMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("Information", "String", "Object[]")));
        InformationMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Information", "String")));
        IsInformationEnabledMethod =
            new Lazy<MethodReference>(
                () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsInformationEnabled")));
        InformationExceptionMethod = new Lazy<MethodReference>(() => ModuleDefinition.ImportReference(
            loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]")));

        WarningFormatMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]")));
        WarningMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Warning", "String")));
        IsWarningEnabledMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsWarningEnabled")));
        WarningExceptionMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]")));

        ErrorFormatMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String", "Object[]")));
        ErrorMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Error", "String")));
        IsErrorEnabledMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsErrorEnabled")));
        ErrorExceptionMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]")));

        FatalFormatMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String", "Object[]")));
        FatalMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("Fatal", "String")));
        IsFatalEnabledMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(loggerTypeDefinition.FindMethod("get_IsFatalEnabled")));
        FatalExceptionMethod = new Lazy<MethodReference>(
            () => ModuleDefinition.ImportReference(
                loggerTypeDefinition.FindMethod("Fatal", "Exception", "String", "Object[]")));
        LoggerType = ModuleDefinition.ImportReference(loggerTypeDefinition);
    }

    public Lazy<MethodReference> TraceFormatMethod;
    public Lazy<MethodReference> TraceMethod;
    public Lazy<MethodReference> TraceExceptionMethod;
    public Lazy<MethodReference> DebugFormatMethod;
    public Lazy<MethodReference> DebugMethod;
    public Lazy<MethodReference> DebugExceptionMethod;
    public Lazy<MethodReference> InformationFormatMethod;
    public Lazy<MethodReference> InformationMethod;
    public Lazy<MethodReference> InformationExceptionMethod;
    public Lazy<MethodReference> WarningFormatMethod;
    public Lazy<MethodReference> WarningMethod;
    public Lazy<MethodReference> WarningExceptionMethod;
    public Lazy<MethodReference> ErrorFormatMethod;
    public Lazy<MethodReference> ErrorMethod;
    public Lazy<MethodReference> ErrorExceptionMethod;
    public Lazy<MethodReference> FatalFormatMethod;
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