using System;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{
	public AssemblyDefinition SerilogReference;

	void FindReference()
	{
		var exsitingReference = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Serilog");

		if (exsitingReference != null)
		{
			SerilogReference = AssemblyResolver.Resolve(exsitingReference);
			return;
		}
		var reference = AssemblyResolver.Resolve("Serilog");
		if (reference != null)
		{
			SerilogReference = reference;
			return;
		}
		throw new Exception("Could not resolve a reference to Serilog.dll.");
	}

}