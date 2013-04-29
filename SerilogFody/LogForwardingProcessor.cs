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

	public void ProcessMethod()
    {
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
        if (methodReference.DeclaringType.FullName != "Anotar.Serilog.Log")
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

                                  Instruction.Create(OpCodes.Ldstr, "{Text:l}"),
                                  Instruction.Create(OpCodes.Ldc_I4_0),
                                  Instruction.Create(OpCodes.Newarr, ModuleWeaver.ModuleDefinition.TypeSystem.Object),
                                  Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference))
                );
            return;
        }
        if (methodReference.IsMatch("Exception", "String", "Object[]"))
        {
            var formatVar = new VariableDefinition(ModuleWeaver.ModuleDefinition.TypeSystem.String);
            var exceptionVar = new VariableDefinition(ModuleWeaver.ExceptionType);
            var paramsVar = new VariableDefinition(ModuleWeaver.ObjectArray);
            Method.Body.Variables.Add(exceptionVar);
            Method.Body.Variables.Add(formatVar);
            Method.Body.Variables.Add(paramsVar);
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, paramsVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, formatVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, exceptionVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, "MethodName"));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, Method.FullName));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition));

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, "LineNumber"));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, lineNumber));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition));


            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, exceptionVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, formatVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, paramsVar));

            instruction.Operand = ModuleWeaver.GetExceptionOperand(methodReference);
            return;
        }
        if (methodReference.IsMatch("String", "Object[]"))
        {
            var messageTemplateVar = new VariableDefinition(ModuleWeaver.ModuleDefinition.TypeSystem.String);
            var paramsVar = new VariableDefinition(ModuleWeaver.ObjectArray);
            Method.Body.Variables.Add(messageTemplateVar);
            Method.Body.Variables.Add(paramsVar);

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, paramsVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc, messageTemplateVar));

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldsfld, Field));

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, "MethodName"));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, Method.FullName));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition));

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, "LineNumber"));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldstr, lineNumber));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldc_I4_0));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Callvirt, ModuleWeaver.forPropertyContextDefinition));

            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, messageTemplateVar));
            processor.InsertBefore(instruction, Instruction.Create(OpCodes.Ldloc, paramsVar));
            instruction.Operand = ModuleWeaver.GetNormalOperand(methodReference);
            return;
        }

        throw new NotImplementedException();
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