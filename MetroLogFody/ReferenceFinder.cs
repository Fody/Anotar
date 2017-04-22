using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public AssemblyDefinition MetroLogReference;

    void FindReference()
    {
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "MetroLog");

        if (existingReference != null)
        {
            MetroLogReference = AssemblyResolver.Resolve(existingReference);
            return;
        }
        var reference = AssemblyResolver.Resolve(new AssemblyNameReference("MetroLog", null));
        if (reference != null)
        {
            MetroLogReference = reference;
            return;
        }
        throw new Exception("Could not resolve a reference to MetroLog.dll.");
    }

}