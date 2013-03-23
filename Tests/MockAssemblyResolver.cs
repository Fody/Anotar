using System;
using System.IO;
using System.Reflection;
using Mono.Cecil;

public class MockAssemblyResolver : IAssemblyResolver
{
    public AssemblyDefinition Resolve(AssemblyNameReference name)
    {
        var fileName = Path.Combine(Directory, name.Name) + ".dll";
        if (File.Exists(fileName))
        {
            return AssemblyDefinition.ReadAssembly(fileName);
        }
        try
        {
            var assembly = Assembly.Load(name.FullName);
            var codeBase = assembly.CodeBase.Replace("file:///", "");
            return AssemblyDefinition.ReadAssembly(codeBase);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {
        throw new NotImplementedException();
    }

    public AssemblyDefinition Resolve(string fullName)
    {
        try
        {
            var codeBase = Assembly.Load(fullName).CodeBase.Replace("file:///", "");

            return AssemblyDefinition.ReadAssembly(codeBase);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
    {
        throw new NotImplementedException();
    }

    public string Directory;
}