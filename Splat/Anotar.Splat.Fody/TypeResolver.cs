using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var locatorType = FindTypeDefinition("Splat.Locator");
        var getLocator = locatorType.Methods.First(x => x.Name == "get_Current");
        GetLocatorMethod = ModuleDefinition.ImportReference(getLocator);

        var dependencyResolver = FindTypeDefinition("Splat.IReadonlyDependencyResolver");
        var getServiceDefinition = dependencyResolver.Methods.First(x => x.Name == "GetService");
        GetServiceMethod = ModuleDefinition.ImportReference(getServiceDefinition);

        var logManagerDefinition = FindTypeDefinition("Splat.ILogManager");
        LogManagerType = ModuleDefinition.ImportReference(logManagerDefinition);
        var getLoggerDefinition = logManagerDefinition.Methods.First(x => x.Name == "GetLogger");
        GetLoggerMethod = ModuleDefinition.ImportReference(getLoggerDefinition);


        var fullLoggerDefinition = FindTypeDefinition("Splat.IFullLogger");
        FullLoggerType = ModuleDefinition.ImportReference(fullLoggerDefinition);

        var loggerDefinition = FindTypeDefinition("Splat.ILogger");

        GetLevelMethod = ModuleDefinition.ImportReference(loggerDefinition.FindMethod("get_Level"));

        DebugMethod = ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Debug", "String"));
        DebugMethodParams =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Debug", "String", "Object[]"));
        DebugExceptionMethod =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("DebugException", "String", "Exception"));
        InfoMethod = ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Info", "String"));
        InfoMethodParams =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Info", "String", "Object[]"));
        InfoExceptionMethod =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("InfoException", "String", "Exception"));
        WarnMethod = ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Warn", "String"));
        WarnMethodParams =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Warn", "String", "Object[]"));
        WarnExceptionMethod =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("WarnException", "String", "Exception"));
        ErrorMethod = ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Error", "String"));
        ErrorMethodParams =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Error", "String", "Object[]"));
        ErrorExceptionMethod =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("ErrorException", "String", "Exception"));
        FatalMethod = ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Fatal", "String"));
        FatalMethodParams =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("Fatal", "String", "Object[]"));
        FatalExceptionMethod =
            ModuleDefinition.ImportReference(fullLoggerDefinition.FindMethod("FatalException", "String", "Exception"));
    }

    public MethodReference GetLoggerMethod;
    public MethodReference GetServiceMethod;
    public TypeReference LogManagerType;
    public MethodReference GetLocatorMethod;
    public MethodReference DebugMethod;
    public MethodReference DebugMethodParams;
    public MethodReference DebugExceptionMethod;
    public MethodReference InfoMethod;
    public MethodReference InfoMethodParams;
    public MethodReference InfoExceptionMethod;
    public MethodReference WarnMethod;
    public MethodReference WarnMethodParams;
    public MethodReference WarnExceptionMethod;
    public MethodReference ErrorMethod;
    public MethodReference ErrorMethodParams;
    public MethodReference ErrorExceptionMethod;
    public MethodReference FatalMethod;
    public MethodReference FatalMethodParams;
    public MethodReference FatalExceptionMethod;
    public TypeReference FullLoggerType;
    public MethodReference GetLevelMethod;
}