using System.Linq;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver
{
    public void Init()
    {
        var logManagerType = FindTypeDefinition("Serilog.Log");
        var logLevel = FindTypeDefinition("Serilog.Events.LogEventLevel");
        VerboseLevel = (int) logLevel.Fields.First(x => x.Name == "Verbose").Constant;
        DebugLevel = (int) logLevel.Fields.First(x => x.Name == "Debug").Constant;
        ErrorLevel = (int) logLevel.Fields.First(x => x.Name == "Error").Constant;
        InformationLevel = (int) logLevel.Fields.First(x => x.Name == "Information").Constant;
        WarningLevel = (int) logLevel.Fields.First(x => x.Name == "Warning").Constant;
        FatalLevel = (int) logLevel.Fields.First(x => x.Name == "Fatal").Constant;

        PublicationOnly = (int) LazyThreadSafetyMode.PublicationOnly;

        var loggerDefinition = FindTypeDefinition("Serilog.ILogger");
        loggerType = ModuleDefinition.ImportReference(loggerDefinition);
        var func = FindTypeDefinition("System.Func`1");
        var funcType = ModuleDefinition.ImportReference(func);

        var genericFunc = funcType.MakeGenericInstanceType(loggerType);

        FuncCtor = ModuleDefinition.ImportReference(genericFunc.Resolve()
                .Methods.First(m => m.IsConstructor && m.Parameters.Count == 2))
            .MakeHostInstanceGeneric(loggerType);

        var lazy = ModuleDefinition.ImportReference(FindTypeDefinition("System.Lazy`1"));
        LazyDefinition = lazy.MakeGenericInstanceType(loggerType);
        var resolvedLazy = LazyDefinition.Resolve();
        LazyCtor =
            ModuleDefinition.ImportReference(resolvedLazy
                    .GetConstructors()
                    .First(m => m.IsMatch("Func`1","LazyThreadSafetyMode")))
                .MakeHostInstanceGeneric(loggerType);
        LazyValue =
            ModuleDefinition.ImportReference(resolvedLazy
                    .Methods
                    .First(m => m.Name=="get_Value"))
                .MakeHostInstanceGeneric(loggerType);

        forContextDefinition =
            ModuleDefinition.ImportReference(
                logManagerType.Methods.First(x => x.Name == "ForContext" && x.HasGenericParameters));


        ForPropertyContextDefinition = ModuleDefinition.ImportReference(loggerDefinition.Methods.First(
            x => x.Name == "ForContext" && !x.IsStatic && !x.HasGenericParameters &&
                 x.IsMatch("String", "Object", "Boolean")));

        IsEnabledMethod =
            ModuleDefinition.ImportReference(loggerDefinition.FindMethod("IsEnabled", "LogEventLevel"));

        verboseMethod =
            ModuleDefinition.ImportReference(loggerDefinition.FindMethod("Verbose", "String", "Object[]"));
        VerboseExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerDefinition.FindMethod("Verbose", "Exception", "String", "Object[]"));
        debugMethod = ModuleDefinition.ImportReference(loggerDefinition.FindMethod("Debug", "String", "Object[]"));
        DebugExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerDefinition.FindMethod("Debug", "Exception", "String", "Object[]"));
        infoMethod =
            ModuleDefinition.ImportReference(loggerDefinition.FindMethod("Information", "String", "Object[]"));
        InfoExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerDefinition.FindMethod("Information", "Exception", "String", "Object[]"));
        warningMethod =
            ModuleDefinition.ImportReference(loggerDefinition.FindMethod("Warning", "String", "Object[]"));
        WarnExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerDefinition.FindMethod("Warning", "Exception", "String", "Object[]"));
        errorMethod = ModuleDefinition.ImportReference(loggerDefinition.FindMethod("Error", "String", "Object[]"));
        ErrorExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerDefinition.FindMethod("Error", "Exception", "String", "Object[]"));
        fatalMethod = ModuleDefinition.ImportReference(loggerDefinition.FindMethod("Fatal", "String", "Object[]"));
        FatalExceptionMethod =
            ModuleDefinition.ImportReference(
                loggerDefinition.FindMethod("Fatal", "Exception", "String", "Object[]"));
    }

    public MethodReference LazyValue;

    public MethodReference FuncCtor;

    public MethodReference LazyCtor;

    public GenericInstanceType LazyDefinition;

    public int PublicationOnly;

    MethodReference debugMethod;
    MethodReference verboseMethod;
    MethodReference infoMethod;
    MethodReference warningMethod;
    MethodReference errorMethod;
    MethodReference fatalMethod;
    TypeReference loggerType;

    public MethodReference VerboseExceptionMethod;
    public MethodReference DebugExceptionMethod;
    public MethodReference InfoExceptionMethod;
    public MethodReference WarnExceptionMethod;
    public MethodReference ErrorExceptionMethod;
    public MethodReference FatalExceptionMethod;

    MethodReference forContextDefinition;
    public MethodReference IsEnabledMethod;
    public int VerboseLevel;
    public int DebugLevel;
    public int FatalLevel;
    public int WarningLevel;
    public int ErrorLevel;
    public int InformationLevel;
    public MethodReference ForPropertyContextDefinition;


}