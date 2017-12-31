using Mono.Cecil.Cil;

public static class ModuleWeaverExtensions
{
    public static Instruction CreateLogExceptionCallInstruction(this ModuleWeaver weaver)
    {
        return Instruction.Create(OpCodes.Call, weaver.WriteExceptionMethod);
    }
}