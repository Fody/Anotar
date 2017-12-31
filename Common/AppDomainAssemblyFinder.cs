using System;
using System.Reflection;

public static class AppDomainAssemblyFinder
{
    public static void Attach()
    {
        AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
    }

    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
        var unqualifiedName = args.Name.Substring(0, args.Name.IndexOf(","));

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.FullName.StartsWith(unqualifiedName + ","))
            {
                return assembly;
            }
        }
        return null;
    }
}