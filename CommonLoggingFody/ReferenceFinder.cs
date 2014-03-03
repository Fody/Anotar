using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition CommonLoggingReference;
	public Lazy<AssemblyDefinition> CommonLoggingCoreReference;

	void FindReference()
	{
        CommonLoggingReference = GetCommonLogging();
        CommonLoggingCoreReference = new Lazy<AssemblyDefinition>(GetCommonCoreLogging);
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

    AssemblyDefinition GetCommonCoreLogging()
    {
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Common.Logging.Core");

        if (existingReference != null)
        {
            return AssemblyResolver.Resolve(existingReference);
        }
        var reference = AssemblyResolver.Resolve("Common.Logging.Core");
        if (reference != null)
        {
            return reference;
        }
        throw new Exception("Could not resolve a reference to Common.Logging.Core.dll.");
    }
}