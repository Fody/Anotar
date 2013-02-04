using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class MetroLogInjector : IInjector
{

    public void Init(AssemblyDefinition reference, ModuleDefinition moduleDefinition)
    {
        var logManagerFactoryType = reference.MainModule.Types.First(x => x.Name == "LogManagerFactory");
        var getDefaultLogManagerDefinition = logManagerFactoryType.Methods.First(x => x.Name == "get_DefaultLogManager");
        getDefaultLogManager = moduleDefinition.Import(getDefaultLogManagerDefinition);

        var logManagerType = reference.MainModule.Types.First(x => x.Name == "ILogManager");
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String", "LoggingConfiguration"));
        buildLoggerMethod = moduleDefinition.Import(getLoggerDefinition);
        var loggerTypeDefinition = reference.MainModule.Types.First(x => x.Name == "ILogger");

        TraceMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Exception"));
        IsTraceEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Exception"));
        DebugMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));
        IsDebugEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));
        InfoMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));
        IsInfoEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));
        WarnMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));
        IsWarnEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));
        ErrorMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));
        IsErrorEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));
        LoggerType = moduleDefinition.Import(loggerTypeDefinition);
    }


    public MethodReference TraceMethod { get; set; }
    public MethodReference IsTraceEnabledMethod { get; set; }
    public MethodReference TraceExceptionMethod { get; set; }
    public MethodReference DebugMethod { get; set; }
    public MethodReference IsDebugEnabledMethod { get; set; }
    public MethodReference DebugExceptionMethod { get; set; }
    public MethodReference InfoMethod { get; set; }
    public MethodReference IsInfoEnabledMethod { get; set; }
    public MethodReference InfoExceptionMethod { get; set; }
    public MethodReference WarnMethod { get; set; }
    public MethodReference IsWarnEnabledMethod { get; set; }
    public MethodReference WarnExceptionMethod { get; set; }
    public MethodReference ErrorMethod { get; set; }
    public MethodReference IsErrorEnabledMethod { get; set; }
    public MethodReference ErrorExceptionMethod { get; set; }

    public TypeReference LoggerType { get; set; }

    MethodReference buildLoggerMethod;
    

    public IAssemblyResolver AssemblyResolver;
    MethodReference getDefaultLogManager;

    public void AddField(TypeDefinition type, MethodDefinition constructor, FieldReference fieldDefinition)
    {
        var instructions = constructor.Body.Instructions;

        instructions.Insert(0, Instruction.Create(OpCodes.Call, getDefaultLogManager));
        instructions.Insert(1, Instruction.Create(OpCodes.Ldstr, type.FullName));
        instructions.Insert(2, Instruction.Create(OpCodes.Ldnull));
        instructions.Insert(3, Instruction.Create(OpCodes.Callvirt, buildLoggerMethod));
        instructions.Insert(4, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
    }

    public string ReferenceName { get { return "MetroLog"; } }
}