﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

class OnExceptionProcessor
{
    public MethodDefinition Method;
    public Action FoundUsageInType;
    public FieldReference LoggerField;
    public ModuleWeaver ModuleWeaver;
    MethodBody body;

    VariableDefinition paramsArrayVariable;
    VariableDefinition messageVariable;
    VariableDefinition exceptionVariable;
    AttributeFinder attributeFinder;

    public void Process()
    {
        attributeFinder = new(Method);
        if (!attributeFinder.Found)
        {
            return;
        }
        Method.ThrowIfIsAsync();

        FoundUsageInType();
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

        exceptionVariable = new(ModuleWeaver.ExceptionType);
        body.Variables.Add(exceptionVariable);
        messageVariable = new(ModuleWeaver.TypeSystem.StringReference);
        body.Variables.Add(messageVariable);
        paramsArrayVariable = new(ModuleWeaver.ObjectArray);
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
            return body.Instructions.First(_ => _.OpCode == OpCodes.Call).Next;
        }
        return body.Instructions.First();
    }

    IEnumerable<Instruction> GetCatchInstructions()
    {
        yield return Instruction.Create(OpCodes.Stloc, exceptionVariable);
        var messageLdstr = Instruction.Create(OpCodes.Ldstr, "");
        yield return messageLdstr;
        yield return Instruction.Create(OpCodes.Ldc_I4, Method.Parameters.Count);
        yield return Instruction.Create(OpCodes.Newarr, ModuleWeaver.TypeSystem.ObjectReference);
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

        if (attributeFinder.FoundVerbose)
        {
            foreach (var instruction in AddWrite(ModuleWeaver.VerboseExceptionMethod, ModuleWeaver.VerboseLevel))
            {
                yield return instruction;
            }
        }
        if (attributeFinder.FoundDebug)
        {
            foreach (var instruction in AddWrite(ModuleWeaver.DebugExceptionMethod, ModuleWeaver.DebugLevel))
            {
                yield return instruction;
            }
        }
        if (attributeFinder.FoundInfo)
        {
            foreach (var instruction in AddWrite(ModuleWeaver.InfoExceptionMethod, ModuleWeaver.InformationLevel))
            {
                yield return instruction;
            }
        }
        if (attributeFinder.FoundWarn)
        {
            foreach (var instruction in AddWrite(ModuleWeaver.WarnExceptionMethod, ModuleWeaver.WarningLevel))
            {
                yield return instruction;
            }
        }
        if (attributeFinder.FoundError)
        {
            foreach (var instruction in AddWrite(ModuleWeaver.ErrorExceptionMethod, ModuleWeaver.ErrorLevel))
            {
                yield return instruction;
            }
        }
        if (attributeFinder.FoundFatal)
        {
            foreach (var instruction in AddWrite(ModuleWeaver.FatalExceptionMethod, ModuleWeaver.FatalLevel))
            {
                yield return instruction;
            }
        }

        yield return Instruction.Create(OpCodes.Rethrow);
    }

    IEnumerable<Instruction> AddWrite(MethodReference writeMethod, int level)
    {
        var sectionNop = Instruction.Create(OpCodes.Nop);

        yield return Instruction.Create(OpCodes.Ldsfld, LoggerField);
        yield return Instruction.Create(OpCodes.Callvirt, ModuleWeaver.LazyValue);
        yield return Instruction.Create(OpCodes.Ldc_I4, level);
        yield return Instruction.Create(OpCodes.Callvirt, ModuleWeaver.IsEnabledMethod);
        yield return Instruction.Create(OpCodes.Brfalse_S, sectionNop);
        yield return Instruction.Create(OpCodes.Ldsfld, LoggerField);
        yield return Instruction.Create(OpCodes.Callvirt, ModuleWeaver.LazyValue);
        yield return Instruction.Create(OpCodes.Ldloc, exceptionVariable);
        yield return Instruction.Create(OpCodes.Ldloc, messageVariable);
        yield return Instruction.Create(OpCodes.Ldnull);
        yield return Instruction.Create(OpCodes.Callvirt, writeMethod);
        yield return sectionNop;
    }
}