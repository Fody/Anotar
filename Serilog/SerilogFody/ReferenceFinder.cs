using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public AssemblyDefinition SerilogReference;

    void FindReference()
    {
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Serilog");

        if (existingReference != null)
        {
            SerilogReference = AssemblyResolver.Resolve(existingReference);
            return;
        }
        var reference = AssemblyResolver.Resolve(new AssemblyNameReference("Serilog", null));
        if (reference != null)
        {
            SerilogReference = reference;
            return;
        }
        throw new Exception("Could not resolve a reference to Serilog.dll.");
    }

}