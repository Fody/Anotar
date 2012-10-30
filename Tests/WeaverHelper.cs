using System.IO;
using System.Reflection;
using Mono.Cecil;

public static class WeaverHelper
{

    public static Assembly Weave(string assemblyPath)
    {
        var newAssembly = assemblyPath.Replace(".dll", "2.dll");
        File.Copy(assemblyPath, newAssembly, true);

        var assemblyResolver = new MockAssemblyResolver
            {
                Directory = Path.GetDirectoryName(assemblyPath)
            };

        var moduleDefinition = ModuleDefinition.ReadModule(newAssembly);
        var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
                AssemblyResolver = assemblyResolver
            };

        weavingTask.Execute();
        moduleDefinition.Write(newAssembly);

        var loadFile = Assembly.LoadFile(newAssembly);
        return loadFile;
    }
}