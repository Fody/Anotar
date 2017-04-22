using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public AssemblyDefinition SplatReference;

    void FindReference()
    {
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Splat");

        if (existingReference != null)
        {
            SplatReference = AssemblyResolver.Resolve(existingReference);
            return;
        }
        var reference = AssemblyResolver.Resolve(new AssemblyNameReference("Splat", null));
        if (reference != null)
        {
            SplatReference = reference;
            return;
        }
        throw new Exception("Could not resolve a reference to Splat.dll.");
    }

}