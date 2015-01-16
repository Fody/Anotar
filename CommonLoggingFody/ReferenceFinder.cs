using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition CommonLoggingReference;
    public AssemblyDefinition CommonLoggingCoreReference;

	void FindReference()
	{
        CommonLoggingReference = GetReference("Common.Logging", "Common.Logging.Portable");
        CommonLoggingCoreReference = GetReference("Common.Logging.Core");
	}

    AssemblyDefinition GetReference(params string[] referenceNames)
    {
        foreach (var referenceName in referenceNames)
        {
            var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == referenceName);

            if (existingReference != null)
            {
                return AssemblyResolver.Resolve(existingReference);
            }
        }
        var message = string.Format("Expected to find a reference to one of {0}.", string.Join(",",referenceNames));
        throw new Exception(message);
    }
}