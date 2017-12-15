using System;
using Mono.Cecil;

public partial class ModuleWeaver
{
    public AssemblyDefinition Log4NetReference;

    void FindReference()
    {
        var reference = AssemblyResolver.Resolve(new AssemblyNameReference("log4net", null));
        if (reference == null)
        {
            throw new Exception("Could not resolve a reference to log4net.dll.");
        }
        Log4NetReference = reference;
    }
}