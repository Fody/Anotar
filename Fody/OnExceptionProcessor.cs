using System;
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
    bool foundInfo;
    bool foundWarn;
    bool foundError;
    public FieldDefinition FieldDefinition;
    public TypeReference ExceptionReference;
    public IInjector Injector;
    MethodBody body;
    public ModuleDefinition ModuleDefinition;

    public MethodReference FormatMethod;
    VariableDefinition paramsArray;
    StringBuilder messageBuilder;
    int messageFormatIndex = 0;
    VariableDefinition messageVariable;
    VariableDefinition exceptionVariable;

    public void Process()
    {
        var customAttributes = Method.CustomAttributes;
        var found = false;
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


        var ret = Instruction.Create(OpCodes.Ret);
        var leave = Instruction.Create(OpCodes.Leave, ret);

        var write = AddCatch();
        body.Instructions.Add(leave);
        body.Instructions.Add(ret);

        var handler = new ExceptionHandler(ExceptionHandlerType.Catch)
            {
                TryStart = FirstInstructionSkipCtor(),
                TryEnd = write,
                HandlerStart = write,
                HandlerEnd = ret,
                CatchType = ExceptionReference,
            };

        body.ExceptionHandlers.Add(handler);

        body.InitLocals = true;
        body.OptimizeMacros();
    }

    Instruction AddCatch()
    {
        exceptionVariable = new VariableDefinition(ExceptionReference);
        body.Variables.Add(exceptionVariable);
        messageVariable = new VariableDefinition(TypeSystem.String);
        body.Variables.Add(messageVariable);
        paramsArray = new VariableDefinition(new ArrayType(TypeSystem.Object));
        body.Variables.Add(paramsArray);
        var startNop = Instruction.Create(OpCodes.Nop);
        body.Instructions.Add(startNop);

        body.Instructions.Add(Instruction.Create(OpCodes.Stloc, exceptionVariable));


        var messageLdstr = Instruction.Create(OpCodes.Ldstr, "");
        body.Instructions.Add(messageLdstr);
        body.Instructions.Add(Instruction.Create(OpCodes.Ldc_I4, Method.Parameters.Count));
        body.Instructions.Add(Instruction.Create(OpCodes.Newarr, TypeSystem.Object));
        body.Instructions.Add(Instruction.Create(OpCodes.Stloc, paramsArray));

        messageBuilder = new StringBuilder(string.Format("Exception occurred in '{0}.{1}'. ", Method.DeclaringType.Name, Method.Name));
        foreach (var parameterDefinition in Method.Parameters)
        {
            ProcessParam(parameterDefinition);
        }
        messageLdstr.Operand = messageBuilder.ToString();

        body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, paramsArray));
        body.Instructions.Add(Instruction.Create(OpCodes.Call, FormatMethod));
        body.Instructions.Add(Instruction.Create(OpCodes.Stloc, messageVariable));

        if (foundDebug)
        {
            AddWrite(Injector.IsDebugEnabledMethod, Injector.DebugExceptionMethod);
        }
        if (foundInfo)
        {
            AddWrite(Injector.IsInfoEnabledMethod, Injector.InfoExceptionMethod);
        }
        if (foundWarn)
        {
            AddWrite(Injector.IsWarnEnabledMethod, Injector.WarnExceptionMethod);
        }
        if (foundError)
        {
            AddWrite(Injector.IsErrorEnabledMethod, Injector.ErrorExceptionMethod);
        }

        body.Instructions.Add(Instruction.Create(OpCodes.Rethrow));
        return startNop;
    }

    void AddWrite(MethodReference isEnabledMethod, MethodReference writeMethod)
    {
        var sectionNop = Instruction.Create(OpCodes.Nop);
        body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, FieldDefinition));
        body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, isEnabledMethod));
        body.Instructions.Add(Instruction.Create(OpCodes.Brfalse_S, sectionNop));
        body.Instructions.Add(Instruction.Create(OpCodes.Ldsfld, FieldDefinition));
        body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, messageVariable));
        body.Instructions.Add(Instruction.Create(OpCodes.Ldloc, exceptionVariable));
        body.Instructions.Add(Instruction.Create(OpCodes.Callvirt, writeMethod));
        body.Instructions.Add(sectionNop);
    }

    void ProcessParam(ParameterDefinition parameterDefinition)
    {

        var paramMetaData = parameterDefinition.ParameterType.MetadataType;
        if (paramMetaData == MetadataType.UIntPtr ||
            paramMetaData == MetadataType.FunctionPointer ||
            paramMetaData == MetadataType.IntPtr ||
            paramMetaData == MetadataType.Pointer)
        {
            return;
        }
        var instructions = body.Instructions;
        instructions.Add(Instruction.Create(OpCodes.Ldloc, paramsArray));
        instructions.Add(Instruction.Create(OpCodes.Ldc_I4, messageFormatIndex));
        instructions.Add(Instruction.Create(OpCodes.Ldarg, parameterDefinition));


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
                    instructions.Add(Instruction.Create(OpCodes.Ldind_I1));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type int16 as int32 on the stack
                case MetadataType.Int16:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_I2));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type int32 as int32 on the stack
                case MetadataType.Int32:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_I4));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type int64 as int64 on the stack
                    // Indirect load value of type unsigned int64 as int64 on the stack (alias for ldind.i8)
                case MetadataType.Int64:
                case MetadataType.UInt64:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_I8));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type unsigned int8 as int32 on the stack
                case MetadataType.Byte:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_U1));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type unsigned int16 as int32 on the stack
                case MetadataType.UInt16:
                case MetadataType.Char:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_U2));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type unsigned int32 as int32 on the stack
                case MetadataType.UInt32:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_U4));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type float32 as F on the stack
                case MetadataType.Single:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_R4));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type float64 as F on the stack
                case MetadataType.Double:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_R8));
                    pointerToValueTypeVariable = true;
                    break;

                    // Indirect load value of type native int as native int on the stack
                case MetadataType.IntPtr:
                case MetadataType.UIntPtr:
                    instructions.Add(Instruction.Create(OpCodes.Ldind_I));
                    pointerToValueTypeVariable = true;
                    break;

                default:
                    // Need to check if it is a value type instance, in which case
                    // we use ldobj instruction to copy the contents of value type
                    // instance to stack and then box it
                    if (referencedTypeSpec.ElementType.IsValueType)
                    {
                        instructions.Add(Instruction.Create(OpCodes.Ldobj, referencedTypeSpec.ElementType));
                        pointerToValueTypeVariable = true;
                    }
                    else
                    {
                        // It is a reference type so just use reference the pointer
                        instructions.Add(Instruction.Create(OpCodes.Ldind_Ref));
                    }
                    break;
            }

            if (pointerToValueTypeVariable)
            {
                // Box the dereferenced parameter type
                instructions.Add(Instruction.Create(OpCodes.Box, referencedTypeSpec.ElementType));
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
                instructions.Add(Instruction.Create(OpCodes.Box, paramType));
            }
        }

        // Store parameter in object[] array
        // ------------------------------------------------------------
        instructions.Add(Instruction.Create(OpCodes.Stelem_Ref));
        messageBuilder.AppendFormat(" {0} '{{{1}}}'", parameterDefinition.Name, messageFormatIndex);

        messageFormatIndex++;
    }


    Instruction FirstInstructionSkipCtor()
    {
        if (Method.IsConstructor && !Method.IsStatic)
        {
            return body.Instructions.Skip(2).First();
        }
        return body.Instructions.First();
    }

}