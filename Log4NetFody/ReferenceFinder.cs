using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition Log4NetReference;

	void FindReference()
	{
		var existingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Log4Net");

		if (existingReference != null)
		{
			Log4NetReference = AssemblyResolver.Resolve(existingReference);
			return;
		}
		var reference = AssemblyResolver.Resolve("Log4Net");
		if (reference != null)
		{
			Log4NetReference = reference;
			return;
		}
		throw new Exception("Could not resolve a reference to Log4Net.dll.");
	}

}