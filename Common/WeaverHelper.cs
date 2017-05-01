using System.IO;
using Mono.Cecil;
using Mono.Cecil.Pdb;

public static class WeaverHelper
{

    public static string Weave(string assemblyPath, string suffix = "2")
    {
        var newAssembly = assemblyPath.Replace(".dll", $".{suffix}.dll");
        var oldPdb = assemblyPath.Replace(".dll", ".pdb");
        var newPdb = assemblyPath.Replace(".dll", $".{suffix}.pdb");
        File.Copy(assemblyPath, newAssembly, true);
        File.Copy(oldPdb, newPdb, true);

        var assemblyResolver = new MockAssemblyResolver
        {
            Directory = Path.GetDirectoryName(assemblyPath)
        };

        using (var symbolStream = File.OpenRead(newPdb))
        {
            var readerParameters = new ReaderParameters
            {
                ReadSymbols = true,
                SymbolStream = symbolStream,
                SymbolReaderProvider = new PdbReaderProvider(),
                AssemblyResolver = assemblyResolver
            };
            var moduleDefinition = ModuleDefinition.ReadModule(assemblyPath, readerParameters);

            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
                AssemblyResolver = assemblyResolver
            };

            weavingTask.Execute();
            moduleDefinition.Write(newAssembly);

            return newAssembly;
        }
    }
}