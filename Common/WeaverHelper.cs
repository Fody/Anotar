using System.IO;
using Mono.Cecil;
using Mono.Cecil.Pdb;

public static class WeaverHelper
{
    public static string GetNewAssemblyPath(string assemblyPath)
    {
        return assemblyPath.Replace(".dll", ".2.dll");
    }

    public static string Weave(string assemblyPath, string newAssembly = null)
    {
        newAssembly = newAssembly ?? GetNewAssemblyPath(assemblyPath);
        var oldPdb = assemblyPath.Replace(".dll", ".pdb");
        var newPdb = assemblyPath.Replace(".dll", ".2.pdb");
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