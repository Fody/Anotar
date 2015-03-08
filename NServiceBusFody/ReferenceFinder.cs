using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition NServiceBusReference;

	void FindReference()
	{
        var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "NServiceBus.Core");

		if (existingReference != null)
		{
			NServiceBusReference = AssemblyResolver.Resolve(existingReference);
			return;
		}
        var reference = AssemblyResolver.Resolve("NServiceBus.Core");
		if (reference != null)
		{
			NServiceBusReference = reference;
			return;
		}
        throw new Exception("Could not resolve a reference to NServiceBus.Core.dll.");
	}

}