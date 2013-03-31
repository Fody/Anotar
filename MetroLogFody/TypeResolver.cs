using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{

    public void Init()
    {
		var logManagerFactoryType = MetroLogReference.MainModule.Types.First(x => x.Name == "LogManagerFactory");
        var getDefaultLogManagerDefinition = logManagerFactoryType.Methods.First(x => x.Name == "get_DefaultLogManager");
        getDefaultLogManager = ModuleDefinition.Import(getDefaultLogManagerDefinition);

		var logManagerType = MetroLogReference.MainModule.Types.First(x => x.Name == "ILogManager");
        var getLoggerDefinition = logManagerType.Methods.First(x => x.Name == "GetLogger" && x.IsMatch("String", "LoggingConfiguration"));
		buildLoggerMethod = ModuleDefinition.Import(getLoggerDefinition);
		var loggerTypeDefinition = MetroLogReference.MainModule.Types.First(x => x.Name == "ILogger");

		TraceMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Exception"));
		isTraceEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsTraceEnabled"));
		TraceExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Trace", "String", "Exception"));
		DebugMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));
		isDebugEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsDebugEnabled"));
		DebugExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Debug", "String", "Exception"));
		InfoMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));
		isInfoEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsInfoEnabled"));
		InfoExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Info", "String", "Exception"));
		WarnMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));
		isWarnEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsWarnEnabled"));
		WarnExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Warn", "String", "Exception"));
		ErrorMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));
		isErrorEnabledMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("get_IsErrorEnabled"));
		ErrorExceptionMethod = ModuleDefinition.Import(loggerTypeDefinition.FindMethod("Error", "String", "Exception"));
		LoggerType = ModuleDefinition.Import(loggerTypeDefinition);
    }



	public IEnumerable<Instruction> GetIsTraceEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isTraceEnabledMethod);
	}

	public MethodReference TraceMethod;
	public MethodReference TraceExceptionMethod;
	public IEnumerable<Instruction> GetIsDebugEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isDebugEnabledMethod);
	}

	public MethodReference DebugMethod;
	public MethodReference DebugExceptionMethod;
	public MethodReference InfoMethod;
	public IEnumerable<Instruction> GetIsInfoEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isInfoEnabledMethod);
	}

	public MethodReference InfoExceptionMethod;
	public MethodReference WarnMethod;
	public IEnumerable<Instruction> GetIsWarnEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isWarnEnabledMethod);
	}

	public MethodReference WarnExceptionMethod;
	public MethodReference ErrorMethod;
	public IEnumerable<Instruction> GetIsErrorEnabledInstructions()
	{
		yield return Instruction.Create(OpCodes.Callvirt, isErrorEnabledMethod);
	}

	public MethodReference ErrorExceptionMethod;

	public TypeReference LoggerType;

    MethodReference buildLoggerMethod;
    

   public MethodReference getDefaultLogManager;
   public MethodReference isTraceEnabledMethod;
   public MethodReference isInfoEnabledMethod;
   public MethodReference isWarnEnabledMethod;
   public MethodReference isErrorEnabledMethod;
   public MethodReference isDebugEnabledMethod;
}