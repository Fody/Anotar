using Mono.Cecil;
using Mono.Cecil.Cil;

public static class ReturnFixer
{
    public static Instruction MakeLastStatementReturn(this MethodDefinition method)
    {
        FixHangingHandlerEnd(method);
        if (method.MethodReturnType.ReturnType.Name == "Void")
        {
            return WithNoReturn(method);
            
        }
        return WithReturnValue(method);
    }

    static void FixHangingHandlerEnd(MethodDefinition method)
    {
        var nopForHandleEnd = Instruction.Create(OpCodes.Nop);
        method.Body.Instructions.Add(nopForHandleEnd);
        foreach (var handler in method.Body.ExceptionHandlers)
        {
            if (handler.HandlerStart != null && handler.HandlerEnd == null)
            {
                handler.HandlerEnd = nopForHandleEnd;
            }
        }
    }

    static Instruction WithReturnValue(MethodDefinition method)
    {
        var returnVariable = new VariableDefinition(method.MethodReturnType.ReturnType);
        method.Body.Variables.Add(returnVariable);
        var instructions = method.Body.Instructions;

        var lastLdloc = Instruction.Create(OpCodes.Ldloc, returnVariable);


        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Ret)
            {
                instructions.Insert(index, Instruction.Create(OpCodes.Stloc, returnVariable));
                instruction.OpCode = OpCodes.Br;
                instruction.Operand = lastLdloc;
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