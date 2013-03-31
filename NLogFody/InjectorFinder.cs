using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition NLogReference;

	void FindReference()
	{
		var exsitingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "NLog");

		if (exsitingReference != null)
		{
			NLogReference = AssemblyResolver.Resolve(exsitingReference);
			return;
		}
		var reference = AssemblyResolver.Resolve("NLog");
		if (reference != null)
		{
			NLogReference = reference;
			return;
		}
		throw new Exception("Could not resolve a refernce to NLog.dll.");
	}

}