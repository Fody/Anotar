using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

public partial class ModuleWeaver
{
    void ProcessType(TypeDefinition type)
    {
        var fieldDefinition = type.Fields.FirstOrDefault(x => x.IsStatic && x.FieldType.FullName == loggerType.FullName);
        Action foundAction;
        if (fieldDefinition == null)
        {
            fieldDefinition = new FieldDefinition("AnotarLogger", FieldAttributes.Static | FieldAttributes.Private, loggerType)
            {
                DeclaringType = type
            };
            foundAction = () => InjectField(type, fieldDefinition);
        }
        else
        {
            foundAction = () => { };
        }

        var fieldReference = fieldDefinition.GetGeneric();
        var foundUsage = false;
        foreach (var method in type.Methods)
        {
            //skip for abstract and delegates
            if (!method.HasBody)
            {
                continue;
            }

            var onExceptionProcessor = new OnExceptionProcessor
            {
                Method = method,
                LoggerField = fieldReference,
                FoundUsageInType = () => foundUsage = true,
                ModuleWeaver = this
            };
            onExceptionProcessor.Process();

            var logForwardingProcessor = new LogForwardingProcessor
            {
                FoundUsageInType = () => foundUsage = true,
                Method = method,
                ModuleWeaver = this,
                LoggerField = fieldReference,
            };
            logForwardingProcessor.ProcessMethod();
        }

        if (foundUsage)
        {
            foundAction();
        }
    }

    void InjectField(TypeDefinition type, FieldDefinition fieldDefinition)
    {
        var staticConstructor = this.GetStaticConstructor(type);
        staticConstructor.Body.SimplifyMacros();
        var genericInstanceMethod = new GenericInstanceMethod(forContextDefinition);
        genericInstanceMethod.GenericArguments.Add(type.GetNonCompilerGeneratedType().GetGeneric());
        var instructions = staticConstructor.Body.Instructions;
        type.Fields.Add(fieldDefinition);

        var returns = instructions.Where(_ => _.OpCode == OpCodes.Ret)
            .ToList();

        var loggerSets = GetLoggerSets(instructions);

        var ilProcessor = staticConstructor.Body.GetILProcessor();

        var fieldReference = fieldDefinition.GetGeneric();
        foreach (var loggerSet in loggerSets)
        {
            ilProcessor.InsertAfter(loggerSet, Instruction.Create(OpCodes.Stsfld, fieldReference));
            ilProcessor.InsertAfter(loggerSet, Instruction.Create(OpCodes.Call, genericInstanceMethod));
        }

        foreach (var returnInstruction in returns)
        {
            var newReturn = Instruction.Create(OpCodes.Ret);
            ilProcessor.InsertAfter(returnInstruction, newReturn);
            ilProcessor.InsertBefore(newReturn, Instruction.Create(OpCodes.Call, genericInstanceMethod));
            ilProcessor.InsertBefore(newReturn, Instruction.Create(OpCodes.Stsfld, fieldReference));
            returnInstruction.OpCode = OpCodes.Nop;
        }

        staticConstructor.Body.OptimizeMacros();
    }

    static IReadOnlyList<Instruction> GetLoggerSets(Collection<Instruction> instructions)
    {
        return instructions.Where(
            _ =>
            {
                if (_.OpCode != OpCodes.Call)
                {
                    return false;
                }

                if (!(_.Operand is MethodReference reference))
                {
                    return false;
                }

                return reference.Name == "set_Logger" &&
                       reference.DeclaringType?.FullName == "Serilog.Log";
            })
            .ToList();
    }
}