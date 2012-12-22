using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public static class ReturnFixer
{
    public static Instruction MakeLastStatementReturn(this MethodDefinition method)
    {
        if (method.MethodReturnType.ReturnType.Name == "Void")
        {
            return WithNoReturn(method);
            
        }
        return WithReturnValue(method);
    }

    static Instruction WithReturnValue(MethodDefinition method)
    {
        var returnVariable = new VariableDefinition(method.MethodReturnType.ReturnType);
        method.Body.Variables.Add(returnVariable);

        var ilProcessor = method.Body.GetILProcessor();
        var instructions = method.Body.Instructions;

        var lastLdloc = Instruction.Create(OpCodes.Ldloc, returnVariable);


        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Ret)
            {
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = lastLdloc;
                ilProcessor.InsertBefore(instruction, Instruction.Create(OpCodes.Stloc,returnVariable));
                index++;
            }
        }
        instructions.Add(lastLdloc);
        instructions.Add(Instruction.Create(OpCodes.Ret));
        return lastLdloc;
    }
    static Instruction WithNoReturn(MethodDefinition method)
    {
        var instructions = method.Body.Instructions;
        var lastReturn = Instruction.Create(OpCodes.Ret);

        //foreach (var exceptionHandler in method.Body.ExceptionHandlers)
        //{
        //    if (exceptionHandler.HandlerEnd == last)
        //    {
        //        exceptionHandler.HandlerEnd = secondLastInstruction;
        //    }
        //}

        foreach (var instruction in instructions)
        {
            if (instruction.OpCode == OpCodes.Ret)
            {
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = lastReturn;
            }
        }
        instructions.Add(lastReturn);
        return lastReturn;
    }
}