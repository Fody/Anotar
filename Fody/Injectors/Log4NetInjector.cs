using System.Collections.Generic;
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

        DebugMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Object"));
        isDebugEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Object", "Exception"));
        InfoMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "Object"));
        isInfoEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "Object", "Exception"));
        WarnMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "Object"));
        isWarnEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "Object", "Exception"));
        ErrorMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Object"));
        isErrorEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Object", "Exception"));
        LoggerType = moduleDefinition.Import(loggerTypeDefinition);
    }


    public MethodReference TraceMethod { get { return DebugMethod; } }
	public IEnumerable<Instruction> GetIsTraceEnabledInstructions() {return GetIsDebugEnabledInstructions(); }
    public MethodReference TraceExceptionMethod { get { return DebugExceptionMethod; } }
    public MethodReference DebugMethod { get; set; }
	public IEnumerable<Instruction> GetIsDebugEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isDebugEnabledMethod);
	}
    public MethodReference DebugExceptionMethod { get; set; }
    public MethodReference InfoMethod { get; set; }
	public IEnumerable<Instruction> GetIsInfoEnabledInstructions ()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isInfoEnabledMethod);
	}
    public MethodReference InfoExceptionMethod { get; set; }
    public MethodReference WarnMethod { get; set; }
	public IEnumerable<Instruction> GetIsWarnEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isWarnEnabledMethod);
	}
    public MethodReference WarnExceptionMethod { get; set; }
    public MethodReference ErrorMethod { get; set; }
	public IEnumerable<Instruction> GetIsErrorEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isErrorEnabledMethod);
	}

	public MethodReference ErrorExceptionMethod { get; set; }

    public TypeReference LoggerType { get; set; }

    MethodReference buildLoggerMethod;
    

    public IAssemblyResolver AssemblyResolver;
	MethodReference isErrorEnabledMethod;
	MethodReference isDebugEnabledMethod;
	MethodReference isInfoEnabledMethod;
	MethodReference isWarnEnabledMethod;

	public void AddField(TypeDefinition type, MethodDefinition constructor, FieldReference fieldDefinition)
    {
        var instructions = constructor.Body.Instructions;

        instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, type.FullName));
        instructions.Insert(1, Instruction.Create(OpCodes.Call, buildLoggerMethod));
        instructions.Insert(2, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
    }

    public string ReferenceName { get { return "Log4Net"; } }
}