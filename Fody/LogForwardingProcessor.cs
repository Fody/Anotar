using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;

public class LogForwardingProcessor
{
    public IInjector Injector;
    public Action<string> LogWarning { get; set; }
    public MethodReference ConcatMethod;
    public MethodReference FormatMethod;
    public TypeReference ExceptionType;
    public TypeReference StringType;
    public ArrayType ObjectArray;
    public MethodDefinition Method;
    public FieldReference Field;
    public Action<bool> FoundUsageInType;
    bool foundUsageInMethod;
    ILProcessor ilProcessor;

    public void ProcessMethod()
    {
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
        if (methodReference.DeclaringType.FullName != "Anotar.Log")
        {
            return;
        }
        if (!foundUsageInMethod)
        {
            Method.Body.InitLocals = true;
            Method.Body.SimplifyMacros();
        }
        foundUsageInMethod = true;
        FoundUsageInType(true);


        var parameters = methodReference.Parameters;

        instruction.OpCode = OpCodes.Callvirt;

        if (parameters.Count == 0)
        {
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));

            var normalOperand = Injector.GetNormalOperand(methodReference);
            //Hack: this should be in the injectors
            if (normalOperand.Parameters.Count == 2)
            {
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldnull));
            }
            instruction.Operand = normalOperand;
        }
        if (methodReference.IsMatch("String"))
        {
            var messageVar = new VariableDefinition(StringType);
            Method.Body.Variables.Add(messageVar);
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));

            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, ConcatMethod));

            var normalOperand = Injector.GetNormalOperand(methodReference);
            //Hack: this should be in the injectors
            if (normalOperand.Parameters.Count == 2)
            {
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldnull));
            }
            instruction.Operand = normalOperand;
        }
        if (methodReference.IsMatch("String", "Exception"))
        {
            var messageVar = new VariableDefinition(StringType);
            var exceptionVar = new VariableDefinition(ExceptionType);
            Method.Body.Variables.Add(exceptionVar);
            Method.Body.Variables.Add(messageVar);
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, exceptionVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, ConcatMethod));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, exceptionVar));

            instruction.Operand = Injector.GetExceptionOperand(methodReference);
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var messageVar = new VariableDefinition(StringType);
            var formatVar = new VariableDefinition(StringType);
            var argsVar = new VariableDefinition(ObjectArray);
            Method.Body.Variables.Add(formatVar);
            Method.Body.Variables.Add(argsVar);
            Method.Body.Variables.Add(messageVar);
            
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, FormatMethod));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, GetMessgaePrefix(instruction)));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageVar));
            ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Call, ConcatMethod));

            var normalOperand = Injector.GetNormalOperand(methodReference);
            //Hack: this should be in the injectors
            if (normalOperand.Parameters.Count == 2)
            {
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldnull));
            }
            instruction.Operand = normalOperand;
        }


    }

    string GetMessgaePrefix(Instruction instruction)
    {
        var sequencePoint = GetPreviousSequencePoint(instruction);
        if (sequencePoint == null)
        {
            return string.Format("Method: '{0}'. ", Method.FullName);
        }

        return string.Format("Method: '{0}'. Line: ~{1}. ", Method.FullName, sequencePoint.StartLine);
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