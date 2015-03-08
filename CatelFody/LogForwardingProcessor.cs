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
        if (methodReference.DeclaringType.FullName != "Anotar.Catel.LogTo")
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
            instruction.Operand = ModuleWeaver.GetLogEnabled(methodReference);
            return;
        }

        instruction.OpCode = OpCodes.Callvirt;

        var logEvent = ModuleWeaver.GetLogEvent(methodReference);
        if (parameters.Count == 0)
        {
            instructions.Replace(instruction, new[]
                                              {
                                                  Instruction.Create(OpCodes.Ldsfld, LoggerField),
                                                  Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)),
                                                  Instruction.Create(OpCodes.Ldnull),
                                                  Instruction.Create(OpCodes.Ldc_I4,logEvent),
                                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.WriteMethod)
                                              });
            return;
        }
        if (methodReference.IsMatch("Exception", "String", "Object[]"))
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
                    Instruction.Create(OpCodes.Stloc, exceptionVar),
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Ldloc, exceptionVar),
                    Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)),
                    Instruction.Create(OpCodes.Ldloc, messageVar),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                    Instruction.Create(OpCodes.Ldloc, paramsVar),
                    Instruction.Create(OpCodes.Call, ModuleWeaver.FormatMethod),
                    Instruction.Create(OpCodes.Ldnull),
                    Instruction.Create(OpCodes.Ldc_I4, logEvent),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.WriteExceptionMethod),
                });
            return;
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var stringInstruction = instruction.FindStringInstruction();

            if (stringInstruction != null)
            {

                var operand = GetMessagePrefix(instruction) + (string) stringInstruction.Operand;
                instructions.Replace(stringInstruction, new[]
                                                        {
                                                            Instruction.Create(OpCodes.Ldsfld, LoggerField),
                                                            Instruction.Create(stringInstruction.OpCode, operand),
                                                        });
                instructions.Replace(instruction, new[]
                                                        {
                                                            Instruction.Create(OpCodes.Call, ModuleWeaver.FormatMethod),
                                                            Instruction.Create(OpCodes.Ldnull),
                                                            Instruction.Create(OpCodes.Ldc_I4, logEvent),
                                                            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.WriteMethod),
                                                        });
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

            instructions.Replace(instruction, new[]
                                              {
                                                  Instruction.Create(OpCodes.Stloc, paramsVar),  
                                                  Instruction.Create(OpCodes.Stloc, messageVar),
                                                  Instruction.Create(OpCodes.Ldsfld, LoggerField),
                                                  Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)),
                                                  Instruction.Create(OpCodes.Ldloc, messageVar),
                                                  Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                                                  Instruction.Create(OpCodes.Ldloc, paramsVar),
                                                  Instruction.Create(OpCodes.Call, ModuleWeaver.FormatMethod),
                                                  Instruction.Create(OpCodes.Ldnull),
                                                  Instruction.Create(OpCodes.Ldc_I4,logEvent),
                                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.WriteMethod),
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