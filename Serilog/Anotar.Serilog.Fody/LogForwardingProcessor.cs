using System;
using System.Collections.Generic;
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
            throw new Exception($"Failed to process '{Method.FullName}'.", exception);
        }
    }

    void ProcessInstruction(Instruction instruction)
    {
        if (!(instruction.Operand is MethodReference methodReference))
        {
            return;
        }

        var declaringTypeName = methodReference.DeclaringType.FullName;
        if (declaringTypeName != "Anotar.Serilog.LogTo")
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
            HandleNoParams(instruction, methodReference);
            return;
        }

        if (methodReference.IsMatch("Exception", "String", "Object[]"))
        {
            HandleExceptionAndStringAndArray(instruction, methodReference);
            return;
        }

        if (methodReference.IsMatch("String", "Object[]"))
        {
            HandleStringAndArray(instruction, methodReference);
            return;
        }

        throw new NotImplementedException();
    }

    void HandleStringAndArray(Instruction instruction, MethodReference methodReference)
    {
        var instructions = Method.Body.Instructions;
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

        var exitNop = Instruction.Create(OpCodes.Nop);

        var replacement = new List<Instruction>
        {
            // store the variables
            Instruction.Create(OpCodes.Stloc, paramsVar),
            Instruction.Create(OpCodes.Stloc, messageVar),

            //Append if
            Instruction.Create(OpCodes.Ldsfld, LoggerField),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.LazyValue),
            Instruction.Create(OpCodes.Ldc_I4, ModuleWeaver.GetLevelForMethodName(methodReference)),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.IsEnabledMethod),
            Instruction.Create(OpCodes.Brfalse, exitNop)
        };

        AppendExtraContext(instruction, replacement);
        replacement.Append(
            //put the variable back on the stack params
            Instruction.Create(OpCodes.Ldloc, messageVar),
            Instruction.Create(OpCodes.Ldloc, paramsVar),
            //call the write method
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference)),
            exitNop
        );
        instructions.Replace(instruction, replacement);
    }

    void HandleExceptionAndStringAndArray(Instruction instruction, MethodReference methodReference)
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

        if (paramsVar == null)
        {
            paramsVar = new VariableDefinition(ModuleWeaver.ObjectArray);
            Method.Body.Variables.Add(paramsVar);
        }

        var exitNop = Instruction.Create(OpCodes.Nop);
        var instructions = Method.Body.Instructions;
        var replacement = new List<Instruction>
        {
            //store variables
            Instruction.Create(OpCodes.Stloc, paramsVar),
            Instruction.Create(OpCodes.Stloc, messageVar),
            Instruction.Create(OpCodes.Stloc, exceptionVar),

            //Append if
            Instruction.Create(OpCodes.Ldsfld, LoggerField),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.LazyValue),
            Instruction.Create(OpCodes.Ldc_I4, ModuleWeaver.GetLevelForMethodName(methodReference)),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.IsEnabledMethod),
            Instruction.Create(OpCodes.Brfalse, exitNop)
        };

        AppendExtraContext(instruction, replacement);
        replacement.Append(
            // put stored variables back on the stack
            Instruction.Create(OpCodes.Ldloc, exceptionVar),
            Instruction.Create(OpCodes.Ldloc, messageVar),
            Instruction.Create(OpCodes.Ldloc, paramsVar),

            //call the write method
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetExceptionOperand(methodReference)),

            //add exit back in
            exitNop
        );
        instructions.Replace(instruction, replacement);
    }

    void AppendExtraContext(Instruction instruction, List<Instruction> replacement)
    {
        //add logger to stack
        replacement.Append(Instruction.Create(OpCodes.Ldsfld, LoggerField));
        replacement.Append(Instruction.Create(OpCodes.Callvirt, ModuleWeaver.LazyValue));
        AppendMethodName(replacement);
        AppendLineNumber(instruction, replacement);
    }

    void HandleNoParams(Instruction instruction, MethodReference methodReference)
    {
        var instructions = Method.Body.Instructions;
        if (methodReference.Name.StartsWith("get_Is"))
        {
            instructions.Replace(instruction,
                new[]
                {
                    Instruction.Create(OpCodes.Ldsfld, LoggerField),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.LazyValue),
                    Instruction.Create(OpCodes.Ldc_I4, ModuleWeaver.GetLevelForIsEnabled(methodReference)),
                    Instruction.Create(OpCodes.Callvirt, ModuleWeaver.IsEnabledMethod)
                });
            return;
        }

        if (paramsVar == null)
        {
            paramsVar = new VariableDefinition(ModuleWeaver.ObjectArray);
            Method.Body.Variables.Add(paramsVar);
        }

        var exitNop = Instruction.Create(OpCodes.Nop);
        var replacement = new List<Instruction>
        {
            //Append if
            Instruction.Create(OpCodes.Ldsfld, LoggerField),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.LazyValue),
            Instruction.Create(OpCodes.Ldc_I4, ModuleWeaver.GetLevelForMethodName(methodReference)),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.IsEnabledMethod),
            Instruction.Create(OpCodes.Brfalse, exitNop)
        };

        AppendExtraContext(instruction, replacement);
        replacement.Append(
            //Write empty array
            Instruction.Create(OpCodes.Ldstr, ""),
            Instruction.Create(OpCodes.Ldc_I4_0),
            Instruction.Create(OpCodes.Newarr, ModuleWeaver.TypeSystem.ObjectReference),

            //call the write method
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.GetNormalOperand(methodReference)),

            exitNop
        );
        instructions.Replace(instruction, replacement);
    }


    void AppendMethodName(List<Instruction> replacement)
    {
        replacement.Append(
            //Write MethodName
            Instruction.Create(OpCodes.Ldstr, "MethodName"),
            Instruction.Create(OpCodes.Ldstr, Method.DisplayName()),
            Instruction.Create(OpCodes.Ldc_I4_0),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.ForPropertyContextDefinition)
        );
    }

    void AppendLineNumber(Instruction instruction, List<Instruction> replacement)
    {
        if (!instruction.TryGetPreviousLineNumber(Method, out var lineNumber))
        {
            return;
        }

        replacement.Append(
            //Write LineNumber
            Instruction.Create(OpCodes.Ldstr, "LineNumber"),
            Instruction.Create(OpCodes.Ldc_I4, lineNumber),
            Instruction.Create(OpCodes.Box, ModuleWeaver.TypeSystem.Int32Reference),
            Instruction.Create(OpCodes.Ldc_I4_0),
            Instruction.Create(OpCodes.Callvirt, ModuleWeaver.ForPropertyContextDefinition)
        );
    }
}