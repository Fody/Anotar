using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

class OnExceptionProcessor
{
    public MethodDefinition Method;
    public TypeSystem TypeSystem;
    public Action<bool> FoundUsageInType;
    bool foundDebug;
    bool foundTrace;
    bool foundInfo;
    bool foundWarn;
    bool foundError;
    public FieldReference Field;
    public TypeReference ExceptionReference;
    public IInjector Injector;
    MethodBody body;

    public MethodReference FormatMethod;
    VariableDefinition paramsArray;
    StringBuilder messageBuilder;
    int messageFormatIndex;
    VariableDefinition messageVariable;
    VariableDefinition exceptionVariable;
    public ModuleWeaver ModuleWeaver;

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


        exceptionVariable = new VariableDefinition(ExceptionReference);
        body.Variables.Add(exceptionVariable);
        messageVariable = new VariableDefinition(TypeSystem.String);
        body.Variables.Add(messageVariable);
        paramsArray = new VariableDefinition(new ArrayType(TypeSystem.Object));
        body.Variables.Add(paramsArray);


        var tryCatchLeaveInstructions = Instruction.Create(OpCodes.Leave, returnFixer.NopBeforeReturn);

        var methodBodyFirstInstruction = GetMethodBodyFirstInstruction();

        var catchInstructions = GetCatchInstructions().ToList();

        ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, tryCatchLeaveInstructions);

        ilProcessor.InsertBefore(returnFixer.NopBeforeReturn, catchInstructions);

        var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                CatchType = ExceptionReference,
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
        yield return Instruction.Create(OpCodes.Newarr, TypeSystem.Object);
        yield return Instruction.Create(OpCodes.Stloc, paramsArray);

        messageBuilder = new StringBuilder(string.Format("Exception occurred in '{0}'. ", Method.FullName));
        foreach (var parameterDefinition in Method.Parameters)
        {
            foreach (var instruction in ProcessParam(parameterDefinition))
            {
                yield return instruction;
            }
        }
        messageLdstr.Operand = messageBuilder.ToString();

        yield return Instruction.Create(OpCodes.Ldloc, paramsArray);
        yield return Instruction.Create(OpCodes.Call, FormatMethod);
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

    IEnumerable<Instruction> ProcessParam(ParameterDefinition parameterDefinition)
    {

        var paramMetaData = parameterDefinition.ParameterType.MetadataType;
        if (paramMetaData == MetadataType.UIntPtr ||
            paramMetaData == MetadataType.FunctionPointer ||
            paramMetaData == MetadataType.IntPtr ||
            paramMetaData == MetadataType.Pointer)
        {
            yield break;
        }
        yield return Instruction.Create(OpCodes.Ldloc, paramsArray);
        yield return Instruction.Create(OpCodes.Ldc_I4, messageFormatIndex);
        yield return Instruction.Create(OpCodes.Ldarg, parameterDefinition);


        // Reset boolean flag variable to false

        // If aparameter is passed by reference then you need to use ldind
        // ------------------------------------------------------------
        var paramType = parameterDefinition.ParameterType;
        if (paramType.IsByReference)
        {
            var referencedTypeSpec = (TypeSpecification) paramType;

            var pointerToValueTypeVariable = false;
            switch (referencedTypeSpec.ElementType.MetadataType)
            {
                    //Indirect load value of type int8 as int32 on the stack
                case MetadataType.Boolean:
                case MetadataType.SByte:
                    yield return Instruction.Create(OpCodes.Ldind_I1);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type int16 as int32 on the stack
                case MetadataType.Int16:
                    yield return Instruction.Create(OpCodes.Ldind_I2);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type int32 as int32 on the stack
                case MetadataType.Int32:
                    yield return Instruction.Create(OpCodes.Ldind_I4);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type int64 as int64 on the stack
                    // Indirect load value of type unsigned int64 as int64 on the stack (alias for ldind.i8)
                case MetadataType.Int64:
                case MetadataType.UInt64:
                    yield return Instruction.Create(OpCodes.Ldind_I8);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type unsigned int8 as int32 on the stack
                case MetadataType.Byte:
                    yield return Instruction.Create(OpCodes.Ldind_U1);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type unsigned int16 as int32 on the stack
                case MetadataType.UInt16:
                case MetadataType.Char:
                    yield return Instruction.Create(OpCodes.Ldind_U2);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type unsigned int32 as int32 on the stack
                case MetadataType.UInt32:
                    yield return Instruction.Create(OpCodes.Ldind_U4);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type float32 as F on the stack
                case MetadataType.Single:
                    yield return Instruction.Create(OpCodes.Ldind_R4);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type float64 as F on the stack
                case MetadataType.Double:
                    yield return Instruction.Create(OpCodes.Ldind_R8);
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type native int as native int on the stack
                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                    yield return Instruction.Create(OpCodes.Ldind_I);
                    pointerToValueTypeVariable = true;
                    break;

                default:
                    // Need to check if it is a value type instance, in which case
                    // we use ldobj instruction to copy the contents of value type
                    // instance to stack and then box it
                    if (referencedTypeSpec.ElementType.IsValueType)
                    {
                        yield return Instruction.Create(OpCodes.Ldobj, referencedTypeSpec.ElementType);
                        pointerToValueTypeVariable = true;
                    }
                    else
                    {
                        // It is a reference type so just use reference the pointer
                        yield return Instruction.Create(OpCodes.Ldind_Ref);
                    }
                    break;
            }

            if (pointerToValueTypeVariable)
            {
                // Box the dereferenced parameter type
                yield return Instruction.Create(OpCodes.Box, referencedTypeSpec.ElementType);
            }

        }
        else
        {

            // If it is a value type then you need to box the instance as we are going 
            // to add it to an array which is of type object (reference type)
            // ------------------------------------------------------------
            if (paramType.IsValueType)
            {
                // Box the parameter type
                yield return Instruction.Create(OpCodes.Box, paramType);
            }
        }

        // Store parameter in object[] array
        // ------------------------------------------------------------
        yield return Instruction.Create(OpCodes.Stelem_Ref);
        messageBuilder.AppendFormat(" {0} '{{{1}}}'", parameterDefinition.Name, messageFormatIndex);

        messageFormatIndex++;
    }


    
}

public static class ILProcessorExtensions
{
    public static void InsertBefore(this ILProcessor processor, Instruction target, IEnumerable<Instruction> instructions)
    {
        foreach (var instruction in instructions)
            processor.InsertBefore(target, instruction);
    }

}