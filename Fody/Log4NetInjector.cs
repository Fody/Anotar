using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class Log4NetInjector : IInjector
{

    public void Init(AssemblyDefinition reference, ModuleDefinition moduleDefinition)
    {
        var logManagerType = reference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        buildLoggerMethod = moduleDefinition.Import(getLoggerDefinition);
       var loggerTypeDefinition = reference.MainModule.Types.First(x => x.Name == "ILog");

        DebugMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod( "Debug", "Object"));
        DebugExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Object", "Exception"));
        InfoMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "Object"));
        InfoExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "Object", "Exception"));
        WarnMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "Object"));
        WarnExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "Object", "Exception"));
        ErrorMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Object"));
        ErrorExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Object", "Exception"));
        LoggerType = moduleDefinition.Import(loggerTypeDefinition);
    }


    public MethodReference DebugMethod { get; set; }
    public MethodReference DebugExceptionMethod { get; set; }
    public MethodReference InfoMethod { get; set; }
    public MethodReference InfoExceptionMethod { get; set; }
    public MethodReference WarnMethod { get; set; }
    public MethodReference WarnExceptionMethod { get; set; }
    public MethodReference ErrorMethod { get; set; }
    public MethodReference ErrorExceptionMethod { get; set; }

    public TypeReference LoggerType { get; set; }

    MethodReference buildLoggerMethod;
    

    public IAssemblyResolver AssemblyResolver;

    public void AddField(TypeDefinition type, MethodDefinition constructor, FieldDefinition fieldDefinition)
    {
        var instructions = constructor.Body.Instructions;

        instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, type.FullName));
        instructions.Insert(1, Instruction.Create(OpCodes.Call, buildLoggerMethod));
        instructions.Insert(2, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
    }

    public string ReferenceName { get { return "Log4Net"; } }
}