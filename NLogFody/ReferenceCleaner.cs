using System.Linq;

public partial class ModuleWeaver
{
    public void RemoveReference()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "Anotar.NLog");
        if (referenceToRemove == null)
        {
            LogInfo("\tNo reference to 'Anotar.NLog.dll' found. References not modified.");
            return;
        }

        ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        LogInfo("\tRemoving reference to 'Anotar.NLog.dll'.");
    }
}