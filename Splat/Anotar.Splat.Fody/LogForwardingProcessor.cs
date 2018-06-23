using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;

public class LogForwardingProcessor
{
    public MethodDefinition Method;
    public FieldReference LoggerField;
    public Action FoundUsageInType;
    bool foundUsageInMethod;
    public ModuleWeaver ModuleWeaver;

    VariableDefinition messageVar;
    VariableDefinition paramsVar;
    VariableDefinition exceptionVar;
    VariableDefinition funcVar;

    public void ProcessMethod()
    {
        Method.CheckForInvalidLogToUsages();
        try
        {
            var instructions = Method.Body.Instructions.Where(x => x.OpCode == OpCodes.Call).ToList();

            foreach (var instruction in instructions)
            {
                ProcessInstruction(instruction);
            }
            if (foundUsageInMethod)
            {
                Method.Body.OptimizeMacros();
            }
        }
        catch (Exception exception)
        {
            throw new Exception($"Failed to process '{Method.FullName}'.", exception);
        }
    }

    void ProcessInstruction(Instruction instruction)
    {
        if (!(instruction.Operand is MethodReference methodReference))
        {
            return;
        }
        if (methodReference.DeclaringType.FullName != "Anotar.Splat.LogTo")
        {
            return;
        }
        if (!foundUsageInMethod)
        {
            Method.Body.InitLocals = true;
            Method.Body.SimplifyMacros();
        }
        foundUsageInMethod = true;
        FoundUsageInType();


        var parameters = methodReference.Parameters;

        var instructions = Method.Body.Instructions;
        if (parameters.Count == 0 && methodReference.Name.StartsWith("get_Is"))
        {
            var enumValue = ModuleWeaver.GetLevel(methodReference);
            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(enumValue),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetLevelMethod),
                    Instruction.Create(OpCodes.Cgt)
                });
            return;
        }
        var isEnabledEnum = ModuleWeaver.GetLevelForLog(methodReference);
        var messagePrefix = GetMessagePrefix(instruction);
        if (parameters.Count == 0)
        {
            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference))
                });
            return;
        }
        instruction.OpCode = OpCodes.Callvirt;
        if (methodReference.IsMatch("String", "Exception"))
        {
            if (messageVar == null)
            {
                messageVar = new VariableDefinition(ModuleWeaver.TypeSystem.StringReference);
                Method.Body.Variables.Add(messageVar);
            }
            if (exceptionVar == null)
            {
                exceptionVar = new VariableDefinition(ModuleWeaver.ExceptionType);
                Method.Body.Variables.Add(exceptionVar);
            }

            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Stloc, exceptionVar),
                    Instruction.Create(OpCodes.Stloc, messageVar),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Ldloc, messageVar),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Ldloc, exceptionVar),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetExceptionOperand(methodReference)),
                });
            return;
        }
        if (methodReference.IsMatch("Func`1", "Exception"))
        {
            if (funcVar == null)
            {
                funcVar = new VariableDefinition(ModuleWeaver.GenericFunc);
                Method.Body.Variables.Add(funcVar);
            }
            if (exceptionVar == null)
            {
                exceptionVar = new VariableDefinition(ModuleWeaver.ExceptionType);
                Method.Body.Variables.Add(exceptionVar);
            }

            var sectionNop = Instruction.Create(OpCodes.Nop);
            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Stloc, exceptionVar),
                    Instruction.Create(OpCodes.Stloc, funcVar),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetLevelMethod),
                    Instruction.Create(isEnabledEnum),
                    Instruction.Create(OpCodes.Bgt_S, sectionNop),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Ldloc, funcVar),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.FuncInvokeMethod),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Ldloc, exceptionVar),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetExceptionOperand(methodReference)),
                    sectionNop
                });
            return;
        }
        if (methodReference.IsMatch("String"))
        {
            if (messageVar == null)
            {
                messageVar = new VariableDefinition(ModuleWeaver.TypeSystem.StringReference);
                Method.Body.Variables.Add(messageVar);
            }

            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Stloc, messageVar),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Ldloc, messageVar),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference)),
                });
            return;
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            if (messageVar == null)
            {
                messageVar = new VariableDefinition(ModuleWeaver.TypeSystem.StringReference);
                Method.Body.Variables.Add(messageVar);
            }
            if (paramsVar == null)
            {
                paramsVar = new VariableDefinition(ModuleWeaver.ObjectArray);
                Method.Body.Variables.Add(paramsVar);
            }

            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Stloc, paramsVar),
                    Instruction.Create(OpCodes.Stloc, messageVar),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Ldloc, messageVar),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Ldloc, paramsVar),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperandParams(methodReference)),
                });
            return;
        }
        if (methodReference.IsMatch("Func`1"))
        {
            if (funcVar == null)
            {
                funcVar = new VariableDefinition(ModuleWeaver.GenericFunc);
                Method.Body.Variables.Add(funcVar);
            }

            var sectionNop = Instruction.Create(OpCodes.Nop);
            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Stloc, funcVar),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetLevelMethod),
                    Instruction.Create(isEnabledEnum),
                    Instruction.Create(OpCodes.Bgt_S, sectionNop),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Ldloc, funcVar),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.FuncInvokeMethod),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference)),
                    sectionNop
                });
            return;
        }
        throw new NotImplementedException();
    }

    string GetMessagePrefix(Instruction instruction)
    {
        //TODO: should prob wrap calls to this method and not concat an empty string. but this will do for now
        if (ModuleWeaver.LogMinimalMessage)
        {
            return string.Empty;
        }

        if (instruction.TryGetPreviousLineNumber(Method, out var lineNumber))
        {
            return $"Method: '{Method.DisplayName()}'. Line: ~{lineNumber}. ";
        }
        return $"Method: '{Method.DisplayName()}'. ";
    }
}