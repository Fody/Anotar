using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition NLogReference;

	void FindReference()
	{
		var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "NLog");

		if (existingReference != null)
		{
			NLogReference = AssemblyResolver.Resolve(existingReference);
			return;
		}
		var reference = AssemblyResolver.Resolve("NLog");
		if (reference != null)
		{
			NLogReference = reference;
			return;
		}
		throw new Exception("Could not resolve a reference to NLog.dll.");
	}

}