using System.IO;
using System.Reflection;
using Mono.Cecil;

public static class WeaverHelper
{

    public static Assembly Weave(string assemblyPath)
    {
        var newAssembly = assemblyPath.Replace(".dll", "2.dll");
        var oldpdb = assemblyPath.Replace(".dll", ".pdb");
        var newpdb = assemblyPath.Replace(".dll", "2.pdb");
        File.Copy(assemblyPath, newAssembly, true);
        File.Copy(oldpdb, newpdb, true);

        var assemblyResolver = new MockAssemblyResolver
            {
                Directory = Path.GetDirectoryName(assemblyPath)
            };

        using (var symbolStream = File.OpenRead(newpdb))
        {
            var readerParameters = new ReaderParameters
                {
                    ReadSymbols = true,
                    SymbolStream = symbolStream
                };
            var moduleDefinition = ModuleDefinition.ReadModule(newAssembly, readerParameters);

            var weavingTask = new ModuleWeaver
                {
                    ModuleDefinition = moduleDefinition,
                    AssemblyResolver = assemblyResolver
                };

            weavingTask.Execute();
            moduleDefinition.Write(newAssembly);

            return Assembly.LoadFile(newAssembly);
        }
    }
}