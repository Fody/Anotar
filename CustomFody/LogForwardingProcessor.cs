using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;

public class LogForwardingProcessor
{
    public MethodDefinition Method;
    public FieldReference Field;
    public Action FoundUsageInType;
    bool foundUsageInMethod;
    ILProcessor processor;
	public ModuleWeaver ModuleWeaver;

    VariableDefinition messageVar;
    VariableDefinition paramsVar;
    VariableDefinition exceptionVar;

    public void ProcessMethod()
    {
        Method.CheckForDynamicUsagesOf("Anotar.Custom.Log");
        try
        {
            processor = Method.Body.GetILProcessor();
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
        if (methodReference.DeclaringType.FullName != "Anotar.Custom.Log")
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

        instruction.OpCode = OpCodes.Callvirt;
        var instructions = Method.Body.Instructions;
        if (parameters.Count == 0)
        {
            instructions.Replace(instruction, new[]
                                              {
                                                  Instruction.Create(OpCodes.Ldsfld, Field),
                                                  Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)),
                                                  Instruction.Create(OpCodes.Ldc_I4_0),
                                                  Instruction.Create(OpCodes.Newarr, ModuleWeaver.ModuleDefinition.TypeSystem.Object),
                                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference))
                                              });
            return;
        }
        if (methodReference.IsMatch( "Exception", "String","Object[]"))
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

            instructions.Replace(instruction, new[]
                                              {
                                                  Instruction.Create(OpCodes.Stloc, paramsVar),
                                                  Instruction.Create(OpCodes.Stloc, messageVar),
                                                  Instruction.Create(OpCodes.Stloc, exceptionVar),
                                                  Instruction.Create(OpCodes.Ldsfld, Field),
                                                  Instruction.Create(OpCodes.Ldloc, exceptionVar),
                                                  Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)),
                                                  Instruction.Create(OpCodes.Ldloc, messageVar),
                                                  Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                                                  Instruction.Create(OpCodes.Ldloc, paramsVar),
                                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetExceptionOperand(methodReference)),
                                              });
            return;
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var stringInstruction = FindStringInstruction(instruction);

            if (stringInstruction != null)
            {

                var operand = GetMessagePrefix(instruction) + (string)stringInstruction.Operand;
                instructions.Replace(stringInstruction, new[]
                                                  {
                                                      Instruction.Create(OpCodes.Ldsfld, Field),
                                                      Instruction.Create(stringInstruction.OpCode, operand),

                                                  });

                instruction.Operand = ModuleWeaver.GetNormalOperand(methodReference);
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
                                                  Instruction.Create(OpCodes.Ldsfld, Field),
                                                  Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)),
                                                  Instruction.Create(OpCodes.Ldloc, messageVar),
                                                  Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod),
                                                  Instruction.Create(OpCodes.Ldloc, paramsVar),
                                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference)),
                                              });
            return;
        }
        throw new NotImplementedException();

    }

    bool IsBasicLogCall(Instruction instruction)
    {
        var previous = instruction.Previous;
        if (previous.OpCode != OpCodes.Newarr || ((TypeReference)previous.Operand).FullName != "System.Object")
            return false;

        previous = previous.Previous;
        if (previous.OpCode != OpCodes.Ldc_I4)
            return false;

        previous = previous.Previous;
        if (previous.OpCode != OpCodes.Ldstr)
            return false;

        return true;
    }

    Instruction FindStringInstruction(Instruction call)
    {
        if (IsBasicLogCall(call))
            return call.Previous.Previous.Previous;

        var previous = call.Previous;
        if (previous.OpCode != OpCodes.Ldloc)
            return null;

        var variable = (VariableDefinition)previous.Operand;

        while (previous != null && (previous.OpCode != OpCodes.Stloc || previous.Operand != variable))
        {
            previous = previous.Previous;
        }

        if (previous == null)
            return null;

        if (IsBasicLogCall(previous))
            return previous.Previous.Previous.Previous;

        return null;
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
            return string.Format("Method: '{0}'. ", Method.FullName);
        }

        return string.Format("Method: '{0}'. Line: ~{1}. ", Method.FullName, sequencePoint.StartLine);
    }

}