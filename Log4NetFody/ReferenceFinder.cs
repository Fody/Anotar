using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition Log4NetReference;

	void FindReference()
	{
		var exsitingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Log4Net");

		if (exsitingReference != null)
		{
			Log4NetReference = AssemblyResolver.Resolve(exsitingReference);
			return;
		}
		var reference = AssemblyResolver.Resolve("Log4Net");
		if (reference != null)
		{
			Log4NetReference = reference;
			return;
		}
		throw new Exception("Could not resolve a refernce to Log4Net.dll.");
	}

}