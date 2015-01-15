using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public Lazy<AssemblyDefinition> CommonLoggingReference;
    public Lazy<AssemblyDefinition> CommonLoggingCoreReference;

	void FindReference()
	{
        CommonLoggingReference = new Lazy<AssemblyDefinition>(() => GetReference("Common.Logging"));
        CommonLoggingCoreReference = new Lazy<AssemblyDefinition>(() => GetReference("Common.Logging.Core"));
	}

    AssemblyDefinition GetReference(string referenceName)
    {
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == referenceName);

        if (existingReference != null)
        {
            return AssemblyResolver.Resolve(existingReference);
        }
        var reference = AssemblyResolver.Resolve(referenceName);
        if (reference != null)
        {
            return reference;
        }
        throw new Exception(string.Format("Could not resolve a reference to {0}.dll.", referenceName));
    }
}