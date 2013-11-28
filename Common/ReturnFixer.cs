﻿using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public class ReturnFixer
{
    public MethodDefinition Method;
    Instruction NopForHandleEnd;
    Collection<Instruction> instructions;
    public Instruction NopBeforeReturn;
    Instruction sealBranchesNop;
    VariableDefinition returnVariable;

    public void MakeLastStatementReturn()
    {

        instructions = Method.Body.Instructions;
        FixHangingHandlerEnd();

        sealBranchesNop = Instruction.Create(OpCodes.Nop);
        instructions.Add(sealBranchesNop);

        NopBeforeReturn = Instruction.Create(OpCodes.Nop);

        if (IsMethodReturnValue())
        {
            returnVariable = new VariableDefinition(Method.MethodReturnType.ReturnType);
            Method.Body.Variables.Add(returnVariable);
        }

        for (var index = 0; index < instructions.Count; index++)
        {
            var operand = instructions[index].Operand as Instruction;
            if (operand != null)
            {
                if (operand.OpCode == OpCodes.Ret)
                {
                    if (IsMethodReturnValue())
                    {
                        // The C# compiler never (AFAICT) jumps directly to a ret
                        // when returning a value from the method. But other Fody
                        // modules and other compilers might. So store the value here.
                        instructions.Insert(index, Instruction.Create(OpCodes.Stloc, returnVariable));
                        instructions.Insert(index, Instruction.Create(OpCodes.Dup));
                        index += 2;
                    }

                    instructions[index].Operand = sealBranchesNop;
                }
            }
        }

        if (!IsMethodReturnValue())
        {
            WithNoReturn();
            return;
        }
        WithReturnValue();
    }

    bool IsMethodReturnValue()
    {
        return Method.MethodReturnType.ReturnType.Name != "Void";
    }

    void FixHangingHandlerEnd()
    {
        if (Method.Body.ExceptionHandlers.Count == 0)
        {
            return;
        }

        NopForHandleEnd = Instruction.Create(OpCodes.Nop);
        Method.Body.Instructions.Add(NopForHandleEnd);
        foreach (var handler in Method.Body.ExceptionHandlers)
        {
            if (handler.HandlerStart != null && handler.HandlerEnd == null)
            {
                handler.HandlerEnd = NopForHandleEnd;
            }
        }
    }


    void WithReturnValue()
    {

        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Ret)
            {
                instructions.Insert(index, Instruction.Create(OpCodes.Stloc, returnVariable));
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = sealBranchesNop;
                index++;
            }
        }
        instructions.Add(NopBeforeReturn);
        instructions.Add(Instruction.Create(OpCodes.Ldloc, returnVariable));
        instructions.Add(Instruction.Create(OpCodes.Ret));

    }

    void WithNoReturn()
    {

        foreach (var instruction in instructions)
        {
            if (instruction.OpCode == OpCodes.Ret)
            {
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = sealBranchesNop;
            }
        }
        instructions.Add(NopBeforeReturn);
        instructions.Add(Instruction.Create(OpCodes.Ret));
    }

}