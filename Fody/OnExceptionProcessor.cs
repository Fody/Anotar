using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

class OnExceptionProcessor
{
    public MethodDefinition Method;
    public Action<bool> FoundUsageInType;
    bool foundDebug;
    bool foundTrace;
    bool foundInfo;
    bool foundWarn;
    bool foundError;
    public FieldReference Field;
	public ModuleWeaver ModuleWeaver { get; set; }
    public IInjector Injector;
    MethodBody body;

    VariableDefinition paramsArrayVariable;
    VariableDefinition messageVariable;
    VariableDefinition exceptionVariable;

    public void Process()
    {
        var customAttributes = Method.CustomAttributes;

        if (customAttributes.Any(_ => _.AttributeType.Name == "AsyncStateMachineAttribute"))
        {
            ModuleWeaver.LogError(string.Format("Could not log exceptions for '{0}'. async methods are not currently supported. Feel free to submit a pull request if you want this feature.", Method.FullName));
            return;
        }

        var found = false;

        if (customAttributes.ContainsAttribute("LogToTraceOnExceptionAttribute"))
        {
            foundTrace = true;
            found = true;
        }
        if (customAttributes.ContainsAttribute("LogToDebugOnExceptionAttribute"))
        {
            foundDebug = true;
            found = true;
        }
        if (customAttributes.ContainsAttribute("LogToInfoOnExceptionAttribute"))
        {
            foundInfo = true;
            found = true;
        }
        if (customAttributes.ContainsAttribute("LogToWarnOnExceptionAttribute"))
        {
            foundWarn = true;
            found = true;
        }
        if (customAttributes.ContainsAttribute("LogToErrorOnExceptionAttribute"))
        {
            foundError = true;
            found = true;
        }

        if (!found)
        {
            return;
        }

        FoundUsageInType(true);
        ContinueProcessing();
    }

    void ContinueProcessing()
    {
        body = Method.Body;

        body.SimplifyMacros();

        var ilProcessor = body.GetILProcessor();

        var returnFixer = new ReturnFixer
            {
                Method = Method
            };
        returnFixer.MakeLastStatementReturn();


		exceptionVariable = new VariableDefinition(ModuleWeaver.ExceptionType);
        body.Variables.Add(exceptionVariable);
		messageVariable = new VariableDefinition(ModuleWeaver.ModuleDefinition.TypeSystem.String);
        body.Variables.Add(messageVariable);
		paramsArrayVariable = new VariableDefinition(new ArrayType(ModuleWeaver.ModuleDefinition.TypeSystem.Object));
        body.Variables.Add(paramsArrayVariable);


        var tryCatchLeaveInstructions = Instruction.Create(OpCodes.Leave, returnFixer.NopBeforeReturn);

        var methodBodyFirstInstruction = GetMethodBodyFirstInstruction();

        var catchInstructions = GetCatchInstructions().ToList();

        ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, tryCatchLeaveInstructions);

        ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, catchInstructions);

        var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = ModuleWeaver.ExceptionType,
                TryStart = methodBodyFirstInstruction,
                TryEnd = tryCatchLeaveInstructions.Next,
                HandlerStart = catchInstructions.First(),
                HandlerEnd = catchInstructions.Last().Next
            };

        body.ExceptionHandlers.Add(handler);

        body.InitLocals = true;
        body.OptimizeMacros();
    }

    Instruction GetMethodBodyFirstInstruction()
    {
        if (Method.IsConstructor)
        {
            return body.Instructions.First(i => i.OpCode == OpCodes.Call).Next;
        }
        return body.Instructions.First();
    }

    IEnumerable<Instruction> GetCatchInstructions()
    {
        yield return Instruction.Create(OpCodes.Stloc, exceptionVariable);


        var messageLdstr = Instruction.Create(OpCodes.Ldstr, "");
        yield return messageLdstr;
        yield return Instruction.Create(OpCodes.Ldc_I4, Method.Parameters.Count);
		yield return Instruction.Create(OpCodes.Newarr, ModuleWeaver.ModuleDefinition.TypeSystem.Object);
        yield return Instruction.Create(OpCodes.Stloc, paramsArrayVariable);

	    var paramsFormatBuilder = new ParamsFormatBuilder(Method, paramsArrayVariable);

	    foreach (var instruction in paramsFormatBuilder.Instructions)
	    {
		    yield return instruction;
	    }
		messageLdstr.Operand = paramsFormatBuilder.MessageBuilder.ToString();

        yield return Instruction.Create(OpCodes.Ldloc, paramsArrayVariable);
		yield return Instruction.Create(OpCodes.Call, ModuleWeaver.FormatMethod);
        yield return Instruction.Create(OpCodes.Stloc, messageVariable);

        if (foundTrace)
        {
            foreach (var instruction in AddWrite(Injector.GetIsTraceEnabledInstructions(), Injector.TraceExceptionMethod))
            {
                yield return instruction;
            }
        }
        if (foundDebug)
        {
			foreach (var instruction in AddWrite(Injector.GetIsDebugEnabledInstructions(), Injector.DebugExceptionMethod))
            {
                yield return instruction;
            }
        }
        if (foundInfo)
        {
			foreach (var instruction in AddWrite(Injector.GetIsInfoEnabledInstructions(), Injector.InfoExceptionMethod))
            {
                yield return instruction;
            }
        }
        if (foundWarn)
        {
			foreach (var instruction in AddWrite(Injector.GetIsWarnEnabledInstructions(), Injector.WarnExceptionMethod))
            {
                yield return instruction;
            }
        }
        if (foundError)
        {
            foreach (var instruction in AddWrite(Injector.GetIsErrorEnabledInstructions(), Injector.ErrorExceptionMethod))
            {
                yield return instruction;
            }
        }

        yield return Instruction.Create(OpCodes.Rethrow);
    }

    IEnumerable<Instruction> AddWrite(IEnumerable<Instruction> isEnabledInstructions, MethodReference writeMethod)
    {
        var sectionNop = Instruction.Create(OpCodes.Nop);
        yield return Instruction.Create(OpCodes.Ldsfld, Field);
		foreach (var intruction in isEnabledInstructions)
	    {
			yield return intruction;
	    }
        yield return Instruction.Create(OpCodes.Brfalse_S, sectionNop);
        yield return Instruction.Create(OpCodes.Ldsfld, Field);
        yield return Instruction.Create(OpCodes.Ldloc, messageVariable);
        yield return Instruction.Create(OpCodes.Ldloc, exceptionVariable);
        yield return Instruction.Create(OpCodes.Callvirt, writeMethod);
        yield return sectionNop;
    }
}
