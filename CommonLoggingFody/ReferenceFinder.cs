using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition CommonLoggingReference;

	void FindReference()
	{
		var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Common.Logging");

		if (existingReference != null)
		{
			CommonLoggingReference = AssemblyResolver.Resolve(existingReference);
			return;
		}
        var reference = AssemblyResolver.Resolve("Common.Logging");
		if (reference != null)
		{
			CommonLoggingReference = reference;
			return;
		}
        throw new Exception("Could not resolve a reference to Common.Logging.dll.");
	}

}