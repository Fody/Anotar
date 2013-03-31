using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition MetroLogReference;

	void FindReference()
	{
		var exsitingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "MetroLog");

		if (exsitingReference != null)
		{
			MetroLogReference = AssemblyResolver.Resolve(exsitingReference);
			return;
		}
		var reference = AssemblyResolver.Resolve("MetroLog");
		if (reference != null)
		{
			MetroLogReference = reference;
			return;
		}
		throw new Exception("Could not resolve a reference to MetroLog.dll.");
	}

}