using Mono.Cecil.Cil;

public static class ModuleWeaverExtensions
{
    public static Instruction CreateLogExceptionCallInstruction(this ModuleWeaver weaver)
    {
        if (weaver.CatelVersion5)
        {
            return Instruction.Create(OpCodes.Call, weaver.WriteExceptionMethod);
        }

        return Instruction.Create(OpCodes.Callvirt, weaver.WriteExceptionMethod);
    }
}