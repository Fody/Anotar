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
    ILProcessor ilProcessor;
	public ModuleWeaver ModuleWeaver;

    VariableDefinition messageVar;
    VariableDefinition paramsVar;
    VariableDefinition exceptionVar;

	public void ProcessMethod()
    {
        Method.CheckForDynamicUsagesOf("Anotar.MetroLog.Log");
        try
        {
            ilProcessor = Method.Body.GetILProcessor();
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
        if (methodReference.DeclaringType.FullName != "Anotar.MetroLog.Log")
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

        if (parameters.Count == 0)
        {
            var fieldAssignment = Instruction.Create(OpCodes.Ldsfld, Field);
            ilProcessor.Replace(instruction, fieldAssignment);
            ilProcessor.InsertAfter(fieldAssignment,
                                    Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)),
                                    Instruction.Create(OpCodes.Ldc_I4_0),
                                    Instruction.Create(OpCodes.Newarr, ModuleWeaver.ModuleDefinition.TypeSystem.Object),
                                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference)));
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

            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, exceptionVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));

            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));
            
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
			ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod));

            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, exceptionVar));
            instruction.Operand = ModuleWeaver.GetExceptionOperand(methodReference);
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var stringInstruction = FindStringInstruction(instruction);

            if (stringInstruction != null)
            {
                stringInstruction.Operand = GetMessagePrefix(instruction) + (string)stringInstruction.Operand;

                ilProcessor.InsertBefore(stringInstruction, Instruction.Create(OpCodes.Ldsfld, Field));

                instruction.Operand = ModuleWeaver.GetNormalOperand(methodReference);
            }
            else
            {
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

                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, paramsVar));
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));

                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));

                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)));
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod));

                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, paramsVar));
                instruction.Operand = ModuleWeaver.GetNormalOperand(methodReference);
            }
        }


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