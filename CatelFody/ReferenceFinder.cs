using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public AssemblyDefinition CatelReference;

    void FindReference()
    {
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Catel.Core");

        if (existingReference != null)
        {
            CatelReference = AssemblyResolver.Resolve(existingReference);
            return;
        }
        var reference = AssemblyResolver.Resolve(new AssemblyNameReference("Catel.Core", null));
        if (reference != null)
        {
            CatelReference = reference;
            return;
        }
        throw new Exception("Could not resolve a reference to Catel.Core.dll.");
    }

}