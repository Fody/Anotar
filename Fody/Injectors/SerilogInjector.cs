using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class SerilogInjector : IInjector
{
    public void Init(AssemblyDefinition reference, ModuleDefinition moduleDefinition)
    {
	    this.moduleDefinition = moduleDefinition;
		//existingLogger = Log.ForContext<ClassWithExistingField>();
		var logManagerType = reference.MainModule.Types.First(x => x.Name == "Log");
		var logEventLevelType = reference.MainModule.Types.First(x => x.Name == "LogEventLevel");
	    debugLevel = (int) logEventLevelType.Fields.First(x => x.Name == "Debug").Constant;
		errorLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Error").Constant;
	    infoLevel = (int) logEventLevelType.Fields.First(x => x.Name == "Information").Constant;
	    warningLevel = (int)logEventLevelType.Fields.First(x => x.Name == "Warning").Constant;

		forContextDefinition = moduleDefinition.Import(logManagerType.Methods.First(x => x.Name == "ForContext" && x.HasGenericParameters && x.IsMatch()));
		


		var loggerTypeDefinition = reference.MainModule.Types.First(x => x.Name == "ILogger");

		isEnabledMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("IsEnabled", "LogEventLevel"));

		DebugMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Object[]"));
		DebugExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "Exception", "String", "Object[]"));
		InfoMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "String", "Object[]"));
		InfoExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Information", "Exception", "String", "Object[]"));
		WarnMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "String", "Object[]"));
		WarnExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Warning", "Exception", "String", "Object[]"));
		ErrorMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Object[]"));
		ErrorExceptionMethod = moduleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "Exception", "String", "Object[]"));
        LoggerType = moduleDefinition.Import(loggerTypeDefinition);
    }


	public IEnumerable<Instruction> GetIsTraceEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Ldc_I4, debugLevel);
		yield return Instruction.Create(OpCodes.Callvirt, isEnabledMethod);
	}

	public MethodReference TraceMethod { get { return DebugMethod; } }
	public MethodReference TraceExceptionMethod { get { return DebugExceptionMethod; } }

	public IEnumerable<Instruction> GetIsDebugEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Ldc_I4, debugLevel);
		yield return Instruction.Create(OpCodes.Callvirt, isEnabledMethod);
	}

	public MethodReference DebugMethod { get; set; }
    public MethodReference DebugExceptionMethod { get; set; }
    public MethodReference InfoMethod { get; set; }
	public IEnumerable<Instruction> GetIsInfoEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Ldc_I4, infoLevel);
		yield return Instruction.Create(OpCodes.Callvirt, isEnabledMethod);
	}

	public MethodReference InfoExceptionMethod { get; set; }
    public MethodReference WarnMethod { get; set; }
	public IEnumerable<Instruction> GetIsWarnEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Ldc_I4, warningLevel);
		yield return Instruction.Create(OpCodes.Callvirt, isEnabledMethod);
	}

	public MethodReference WarnExceptionMethod { get; set; }
    public MethodReference ErrorMethod { get; set; }
	public IEnumerable<Instruction> GetIsErrorEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Ldc_I4, errorLevel);
		yield return Instruction.Create(OpCodes.Callvirt, isEnabledMethod);
	}

	public MethodReference ErrorExceptionMethod { get; set; }

    public TypeReference LoggerType { get; set; }

    public IAssemblyResolver AssemblyResolver;
	MethodReference forContextDefinition;
	ModuleDefinition moduleDefinition;
	MethodReference isEnabledMethod;
	int debugLevel;
	int warningLevel;
	int errorLevel;
	int infoLevel;

	public void AddField(TypeDefinition type, MethodDefinition constructor, FieldReference fieldDefinition)
	{
		var genericInstanceMethod = new GenericInstanceMethod(forContextDefinition);
		genericInstanceMethod .GenericArguments.Add(type);
		var instructions = constructor.Body.Instructions;
		instructions.Insert(0, Instruction.Create(OpCodes.Call, genericInstanceMethod));
		instructions.Insert(1, Instruction.Create(OpCodes.Stsfld, fieldDefinition));
	}

	public string ReferenceName { get { return "Serilog"; } }
}