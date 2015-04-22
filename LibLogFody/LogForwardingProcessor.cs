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
            throw new Exception(string.Format("Failed to process '{0}'.", Method.FullName), exception);
        }
    }

    void ProcessInstruction(Instruction instruction)
    {
        var methodReference = instruction.Operand as MethodReference;
        if (methodReference == null)
        {
            return;
        }
        if (methodReference.DeclaringType.FullName != "Anotar.LibLog.LogTo")
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
            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.GetLogEnabled(methodReference))
                });
            return;
        }
        var messagePrefix = GetMessagePrefix(instruction);
        var isEnabledMethod = ModuleWeaver.GetLogEnabledForLog(methodReference);

        if (parameters.Count == 0)
        {
            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.GetNormalOperand(methodReference))
                });
            return;
        }
        if (methodReference.IsMatch("String", "Exception"))
        {
            if (messageVar == null)
            {
                messageVar = new VariableDefinition(ModuleWeaver.ModuleDefinition.TypeSystem.String);
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
                    Instruction.Create(OpCodes.Ldc_I4_0),
                    Instruction.Create(OpCodes.Newarr, ModuleWeaver.ModuleDefinition.TypeSystem.Object),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.GetExceptionOperand(methodReference)),
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
                    Instruction.Create(OpCodes.Call, isEnabledMethod),
                    Instruction.Create(OpCodes.Brfalse_S, sectionNop),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Ldloc, funcVar),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.FuncInvokeMethod),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Ldloc, exceptionVar),
                    Instruction.Create(OpCodes.Ldc_I4_0),
                    Instruction.Create(OpCodes.Newarr, ModuleWeaver.ModuleDefinition.TypeSystem.Object),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.GetExceptionOperand(methodReference)),
                    sectionNop
                });
            return;
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var stringInstruction = instruction.FindStringInstruction();

            if (stringInstruction != null)
            {
                var operand = messagePrefix + (string) stringInstruction.Operand;
                instructions.Replace(stringInstruction,
                    new[]
                    {
                        Instruction.Create(OpCodes.Ldsfld, LoggerField),
                        Instruction.Create(stringInstruction.OpCode, operand),
                    });

                instruction.Operand = ModuleWeaver.GetNormalFormatOperand(methodReference);
                return;
            }
            if (messageVar == null)
            {
                messageVar = new VariableDefinition(ModuleWeaver.ModuleDefinition.TypeSystem.String);
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
                    Instruction.Create(OpCodes.Call, ModuleWeaver.GetNormalFormatOperand(methodReference)),
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
                    Instruction.Create(OpCodes.Call, isEnabledMethod),
                    Instruction.Create(OpCodes.Brfalse_S, sectionNop),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldstr, messagePrefix),
                    Instruction.Create(OpCodes.Ldloc, funcVar),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.FuncInvokeMethod),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.GetNormalFormatOperand(methodReference)),
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
        var sequencePoint = instruction.GetPreviousSequencePoint();
        if (sequencePoint == null)
        {
            return string.Format("Method: '{0}'. ", Method.DisplayName());
        }

        return string.Format("Method: '{0}'. Line: ~{1}. ", Method.DisplayName(), sequencePoint.StartLine);
    }

}