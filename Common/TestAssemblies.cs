using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

public class TestAssemblies
{
    static object weaverLock = new object();
    static IDictionary<string, Tuple<Assembly, string, string>> assemblies = new Dictionary<string, Tuple<Assembly, string, string>>();
    string assemblyToProcessName;

    public TestAssemblies(string assemblyToProcessName)
    {
        this.assemblyToProcessName = assemblyToProcessName;
    }

    public Assembly GetAssembly(string target)
    {
        WeaveAssembly(target);
        return assemblies[target].Item1;
    }

    public string GetBeforePath(string target)
    {
        WeaveAssembly(target);
        return assemblies[target].Item2;
    }

    public string GetAfterPath(string target)
    {
        WeaveAssembly(target);
        return assemblies[target].Item3;
    }

    private void WeaveAssembly(string target)
    {
        lock (weaverLock)
        {
            if (assemblies.ContainsKey(target) == false)
            {
                AppDomainAssemblyFinder.Attach();
                var assemblyPathUri = new Uri(new Uri(typeof(TestAssemblies).GetTypeInfo().Assembly.CodeBase), $"../../../../{assemblyToProcessName}/bin/Debug/{target}/{assemblyToProcessName}.dll");
                var beforeAssemblyPath = Path.GetFullPath(assemblyPathUri.LocalPath);
#if (!DEBUG)
                beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
                var afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
                assemblies[target] = Tuple.Create(Assembly.LoadFile(afterAssemblyPath), beforeAssemblyPath, afterAssemblyPath);
            }
        }
    }
}
