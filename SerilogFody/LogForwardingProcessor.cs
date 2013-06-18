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
	    Method.CheckForDynamicUsagesOf("Anotar.Serilog.LogTo");
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
        var declaringTypeName = methodReference.DeclaringType.FullName;
        if (declaringTypeName != "Anotar.Serilog.Log" && declaringTypeName != "Anotar.Serilog.LogTo")
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

        var lineNumber = GetLineNumber(instruction);
        if (parameters.Count == 0)
        {
            var paramsVar = new VariableDefinition(ModuleWeaver.ObjectArray);
            Method.Body.Variables.Add(paramsVar);

            var fieldAssignment = Instruction.Create(OpCodes.Ldsfld, Field);
            processor.Replace(instruction, fieldAssignment);
            processor.InsertAfter(fieldAssignment,

                                  Instruction.Create(OpCodes.Ldstr, "MethodName"),
                                  Instruction.Create(OpCodes.Ldstr, Method.FullName),
                                  Instruction.Create(OpCodes.Ldc_I4_0),
                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition),

                                  Instruction.Create(OpCodes.Ldstr, "LineNumber"),
                                  Instruction.Create(OpCodes.Ldstr, lineNumber),
                                  Instruction.Create(OpCodes.Ldc_I4_0),
                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition),

                                  Instruction.Create(OpCodes.Ldstr, ""),
                                  Instruction.Create(OpCodes.Ldc_I4_0),
                                  Instruction.Create(OpCodes.Newarr, ModuleWeaver.ModuleDefinition.TypeSystem.Object),
                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference))
                );
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

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, paramsVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, exceptionVar));

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));

            AddPropertyContexts(instruction, lineNumber);

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, exceptionVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, paramsVar));

            instruction.Operand = ModuleWeaver.GetExceptionOperand(methodReference);
            return;
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var stringInstruction = FindStringInstruction(instruction);

            if (stringInstruction != null)
            {
                processor.InsertBefore(stringInstruction, Instruction.Create(OpCodes.Ldsfld, Field));

                AddPropertyContexts(stringInstruction, lineNumber);

                instruction.Operand = ModuleWeaver.GetNormalOperand(methodReference);
                return;
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

                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, paramsVar));
                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));

                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));

                AddPropertyContexts(instruction, lineNumber);

                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
                processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, paramsVar));
                instruction.Operand = ModuleWeaver.GetNormalOperand(methodReference);
                return;
            }
        }

        throw new NotImplementedException();
    }

    private void AddPropertyContexts(Instruction instruction, string lineNumber)
    {
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, "MethodName"));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, Method.FullName));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition));

        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, "LineNumber"));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, lineNumber));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
        processor.InsertBefore(instruction, Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition));
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

    string GetLineNumber(Instruction instruction)
    {
        var sequencePoint = instruction.GetPreviousSequencePoint();
        if (sequencePoint == null)
        {
            return null;
        }

        return sequencePoint.StartLine.ToString();
    }

}