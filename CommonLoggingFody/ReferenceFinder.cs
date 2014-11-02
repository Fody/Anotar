using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public Lazy<AssemblyDefinition> CommonLoggingReference;

	void FindReference()
	{
        CommonLoggingReference = new Lazy<AssemblyDefinition>(GetCommonLogging);
	}

    AssemblyDefinition GetCommonLogging()
    {
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Common.Logging");

        if (existingReference != null)
        {
            return AssemblyResolver.Resolve(existingReference);
        }
        var reference = AssemblyResolver.Resolve("Common.Logging");
        if (reference != null)
        {
            return reference;
        }
        throw new Exception("Could not resolve a reference to Common.Logging.dll.");
    }
}