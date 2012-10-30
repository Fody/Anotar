using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class NLogInjector : IInjector
{
    public bool IsCompat(ModuleDefinition moduleDefinition)
    {
        var exsitingReference = moduleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Nlog");
        AssemblyDefinition reference;
        if (exsitingReference == null)
        {
            reference = moduleDefinition.AssemblyResolver.Resolve("Nlog");
            if (reference == null)
            {
                return false;
            }
        }
        else
        {
            reference = AssemblyResolver.Resolve(exsitingReference);
        }
        moduleDefinition.AssemblyReferences.Add(reference.Name);
        var logManagerType = reference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        BuildLoggerMethod = moduleDefinition.Import(getLoggerDefinition);
        var getLoggerGenericDefinition = logManagerType.Methods.First(x => x.Name == "GetCurrentClassLogger");
        BuildLoggerGenericMethod = moduleDefinition.Import(getLoggerGenericDefinition);
        var loggerTypeDefinition = reference.MainModule.Types.First(x => x.Name == "Logger");

        DebugStringMethod = moduleDefinition.Import(loggerTypeDefinition.Methods.First(x => x.Name == "Debug" && x.IsMatch("String")));
        LoggerType = moduleDefinition.Import(loggerTypeDefinition);

        return true;
    }

    public MethodReference DebugMethod { get; set; }
    public MethodReference DebugStringMethod { get; set; }
    public MethodReference DebugParamsMethod { get; set; }
    public MethodReference DebugStringExceptionMethod { get; set; }
    public MethodReference InfoMethod { get; set; }
    public MethodReference InfoStringMethod { get; set; }
    public MethodReference InfoParamsMethod { get; set; }
    public MethodReference InfoStringExceptionMethod { get; set; }
    public MethodReference WarnMethod { get; set; }
    public MethodReference WarnStringMethod { get; set; }
    public MethodReference WarnParamsMethod { get; set; }
    public MethodReference WarnStringExceptionMethod { get; set; }
    public MethodReference ErrorMethod { get; set; }
    public MethodReference ErrorStringMethod { get; set; }
    public MethodReference ErrorParamsMethod { get; set; }
    public MethodReference ErrorStringExceptionMethod { get; set; }

    public TypeReference LoggerType { get; set; }
    
    public MethodReference BuildLoggerMethod { get; set; }
    public MethodReference BuildLoggerGenericMethod { get; set; }
    

    public IAssemblyResolver AssemblyResolver;

    public void AddField(TypeDefinition type, MethodDefinition constructor, FieldDefinition fieldDefinition)
    {
        var instructions = constructor.Body.Instructions;

        if (type.HasGenericParameters)
        {
            instructions.Insert(0, Instruction.Create(OpCodes.Call, BuildLoggerGenericMethod));
            instructions.Insert(1, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
        }
        else
        {
            instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, type.FullName));
            instructions.Insert(1, Instruction.Create(OpCodes.Call, BuildLoggerMethod));
            instructions.Insert(2, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
        }

    }

}