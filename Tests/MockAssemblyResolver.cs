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
        var codeBase = Assembly.Load(name.FullName).CodeBase.Replace("file:///", "");
        return AssemblyDefinition.ReadAssembly(codeBase);
    }

    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    {

        throw new NotImplementedException();
    }

    public AssemblyDefinition Resolve(string fullName)
    {
        var codeBase = Assembly.Load(fullName).CodeBase.Replace("file:///","");

        return AssemblyDefinition.ReadAssembly(codeBase);
    }

    public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
    {
        throw new NotImplementedException();
    }

    public string Directory;
}