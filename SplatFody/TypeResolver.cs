using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public void Init()
    {
        var locatorType = SplatReference.MainModule.Types.First(x => x.Name == "Locator");
        var getLocator = locatorType.Methods.First(x => x.Name == "get_Current");
        GetLocatorMethod = ModuleDefinition.Import(getLocator);

        var mscorlib = AssemblyResolver.Resolve("mscorlib");
        var typeType = mscorlib.MainModule.Types.First(x => x.Name == "Type");
        GetTypeFromHandle = typeType.Methods
            .First(x => x.Name == "GetTypeFromHandle" &&
                        x.Parameters.Count == 1 &&
                        x.Parameters[0].ParameterType.Name == "RuntimeTypeHandle");
        GetTypeFromHandle = ModuleDefinition.Import(GetTypeFromHandle);


        var dependencyResolver = SplatReference.MainModule.Types.First(x => x.Name == "IDependencyResolver");
        var getServiceDefinition = dependencyResolver.Methods.First(x => x.Name == "GetService");
        GetServiceMethod = ModuleDefinition.Import(getServiceDefinition);

        var logManagerDefinition = SplatReference.MainModule.Types.First(x => x.Name == "ILogManager");
        LogManagerType = ModuleDefinition.Import(logManagerDefinition);
        var getLoggerDefinition = logManagerDefinition.Methods.First(x => x.Name == "GetLogger");
        GetLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);



        var fullLoggerDefinition = SplatReference.MainModule.Types.First(x => x.Name == "IFullLogger");
        FullLoggerType = ModuleDefinition.Import(fullLoggerDefinition);


        var loggerDefinition = SplatReference.MainModule.Types.First(x => x.Name == "ILogger");
        //FullLoggerType = ModuleDefinition.Import(loggerDefinition);

        getLevelMethod = ModuleDefinition.Import(loggerDefinition.FindMethod("get_Level"));

        DebugMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Debug", "String"));
        DebugMethodParams = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Debug", "String", "Object[]"));
        DebugExceptionMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("DebugException", "String", "Exception"));
        InfoMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Info", "String"));
        InfoMethodParams = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Info", "String", "Object[]"));
        InfoExceptionMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("InfoException", "String", "Exception"));
        WarnMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Warn", "String"));
        WarnMethodParams = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Warn", "String", "Object[]"));
        WarnExceptionMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("WarnException", "String", "Exception"));
        ErrorMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Error", "String"));
        ErrorMethodParams = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Error", "String", "Object[]"));
        ErrorExceptionMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("ErrorException", "String", "Exception"));
        FatalMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Fatal", "String"));
        FatalMethodParams = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("Fatal", "String", "Object[]"));
        FatalExceptionMethod = ModuleDefinition.Import(fullLoggerDefinition.FindMethod("FatalException", "String", "Exception"));
	
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
	public MethodReference getLevelMethod;
    public MethodReference GetTypeFromHandle;
}