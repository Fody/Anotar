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

	public void ProcessMethod()
    {
        Method.CheckForDynamicUsagesOf("Anotar.Log4Net.Log");
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
        if (methodReference.DeclaringType.FullName != "Anotar.Log4Net.Log")
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
			var messageVar = new VariableDefinition(ModuleWeaver.ModuleDefinition.TypeSystem.String);
			var exceptionVar = new VariableDefinition(ModuleWeaver.ExceptionType);
            Method.Body.Variables.Add(exceptionVar);
            Method.Body.Variables.Add(messageVar);
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, exceptionVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
			ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod));
			ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));
			ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, exceptionVar));
            instruction.Operand = ModuleWeaver.GetExceptionOperand(methodReference);
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var formatVar = new VariableDefinition(ModuleWeaver.ModuleDefinition.TypeSystem.String);
            var paramsVar = new VariableDefinition(ModuleWeaver.ObjectArray);
            Method.Body.Variables.Add(formatVar);
            Method.Body.Variables.Add(paramsVar);


            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, paramsVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, formatVar));
            
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessagePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, formatVar));
			ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, ModuleWeaver.ConcatMethod));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, formatVar));

            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, formatVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, paramsVar));
            instruction.Operand = ModuleWeaver.GetNormalOperand(methodReference);
        }


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