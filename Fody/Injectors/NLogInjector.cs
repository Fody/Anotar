using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class NLogInjector : IInjector
{
    public void Init(AssemblyDefinition reference, ModuleDefinition moduleDefinition)
    {
        var logManagerType = reference.MainModule.Types.First(x => x.Name == "LogManager");
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String"));
        buildLoggerMethod = moduleDefinition.Import(getLoggerDefinition);
        var getLoggerGenericDefinition = logManagerType.Methods.First(x => x.Name == "GetCurrentClassLogger");
        buildLoggerGenericMethod = moduleDefinition.Import(getLoggerGenericDefinition);
        var loggerTypeDefinition = reference.MainModule.Types.First(x => x.Name == "Logger");

        TraceMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String"));
        isTraceEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
        TraceExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("TraceException", "String", "Exception"));
        DebugMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String"));
        isDebugEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
        DebugExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("DebugException", "String", "Exception"));
        InfoMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String"));
        isInfoEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
        InfoExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("InfoException", "String", "Exception"));
        WarnMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String"));
        isWarnEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
        WarnExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("WarnException", "String", "Exception"));
        ErrorMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String"));
        isErrorEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
        ErrorExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("ErrorException", "String", "Exception"));
        LoggerType = moduleDefinition.Import(loggerTypeDefinition);
    }

	public void InjectNormalMethod(Instruction instruction, ILProcessor ilProcessor, VariableDefinition messageVar)
	{
		throw new System.NotImplementedException();
	}

	public IEnumerable<Instruction> GetIsTraceEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isTraceEnabledMethod);
	}

	public MethodReference TraceMethod { get; set; }
    public MethodReference TraceExceptionMethod { get; set; }
	public IEnumerable<Instruction> GetIsDebugEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isDebugEnabledMethod);
	}

	public MethodReference DebugMethod { get; set; }
    public MethodReference DebugExceptionMethod { get; set; }
    public MethodReference InfoMethod { get; set; }
	public IEnumerable<Instruction> GetIsInfoEnabledInstructions()
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
    MethodReference buildLoggerGenericMethod;
    

    public IAssemblyResolver AssemblyResolver;
	MethodReference isTraceEnabledMethod;
	MethodReference isDebugEnabledMethod;
	MethodReference isInfoEnabledMethod;
	MethodReference isWarnEnabledMethod;
	MethodReference isErrorEnabledMethod;

	public void AddField(TypeDefinition type, MethodDefinition constructor, FieldReference fieldDefinition)
    {
        var instructions = constructor.Body.Instructions;

        if (type.HasGenericParameters)
        {
            instructions.Insert(0, Instruction.Create(OpCodes.Call, buildLoggerGenericMethod));
            instructions.Insert(1, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
        }
        else
        {
            instructions.Insert(0, Instruction.Create(OpCodes.Ldstr, type.FullName));
            instructions.Insert(1, Instruction.Create(OpCodes.Call, buildLoggerMethod));
            instructions.Insert(2, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
        }

    }

    public string ReferenceName { get { return "NLog"; } }
}