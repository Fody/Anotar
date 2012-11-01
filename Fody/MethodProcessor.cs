using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;

public class MethodProcessor
{
    public IInjector injector;
    public Action<string> LogWarning { get; set; }
    public MethodReference concatMethod;
    public MethodReference formatMethod;
    public TypeReference exceptionType;
    public TypeReference stringType;
    public ArrayType objectArray;
    public MethodDefinition method;
    public FieldDefinition fieldDefinition;
    public Action<bool> FoundUsageInType;
    bool foundUsageInMethod;
    ILProcessor ilProcessor;

    public void ProcessMethod()
    {
         ilProcessor = method.Body.GetILProcessor();
        var instructions = method.Body.Instructions.Where(x => x.OpCode == OpCodes.Call).ToList();
       
        foreach (var instruction in instructions)
        {
           ProcessInstruction(instruction);
        }
        if (foundUsageInMethod)
        {
            method.Body.OptimizeMacros();
        }
    }

    void ProcessInstruction(Instruction instruction)
    {
        var methodReference = instruction.Operand as MethodReference;
        if (methodReference == null)
        {
            return;
        }
        if (methodReference.DeclaringType.FullName != "Anotar.Log")
        {
            return;
        }
        if (!foundUsageInMethod)
        {
            method.Body.InitLocals = true;
            method.Body.SimplifyMacros();
        }
        foundUsageInMethod = true;
        FoundUsageInType(true);


        var parameters = methodReference.Parameters;

        instruction.OpCode = OpCodes.Callvirt;

        if (parameters.Count == 0)
        {
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, fieldDefinition));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));

            instruction.Operand = injector.GetNormalOperand(methodReference);
        }
        if (methodReference.IsMatch("String"))
        {
            var messageVar = new VariableDefinition(stringType);
            method.Body.Variables.Add(messageVar);
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));

            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, fieldDefinition));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, concatMethod));

            instruction.Operand = injector.GetNormalOperand(methodReference);
        }
        if (methodReference.IsMatch("String", "Exception"))
        {
            var messageVar = new VariableDefinition(stringType);
            var exceptionVar = new VariableDefinition(exceptionType);
            method.Body.Variables.Add(exceptionVar);
            method.Body.Variables.Add(messageVar);
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, exceptionVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));


            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, fieldDefinition));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, concatMethod));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, exceptionVar));

            instruction.Operand = injector.GetExceptionOperand(methodReference);
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var messageVar = new VariableDefinition(stringType);
            var formatVar = new VariableDefinition(stringType);
            var argsVar = new VariableDefinition(objectArray);
            method.Body.Variables.Add(formatVar);
            method.Body.Variables.Add(argsVar);
            method.Body.Variables.Add(messageVar);
            
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, formatMethod));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, fieldDefinition));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, concatMethod));

            instruction.Operand = injector.GetNormalOperand(methodReference);
        }


    }

    string GetMessgaePrefix(Instruction instruction)
    {
        var sequencePoint = GetPreviousSequencePoint(instruction);
        if (sequencePoint == null)
        {
            return string.Format("Method: {0}. ", method.Name);
        }

        return string.Format("Method: {0}. Line: ~{1}. ", method.Name, sequencePoint.StartLine);
    }

    static SequencePoint GetPreviousSequencePoint(Instruction instruction)
    {
        while (true)
        {

            if (instruction.SequencePoint != null)
            {
                return instruction.SequencePoint;
            }

            instruction = instruction.Previous;
            if (instruction == null)
            {
                return null;
            }
        }
    }
}