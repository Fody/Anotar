using System;
using System.Reflection;
using Scalpel;

[Remove]
public static class AssemblyExtensions
{

    public static dynamic GetInstance(this Assembly assembly, string className)
    {
        var type = assembly.GetType(className, true);
        //dynamic instance = FormatterServices.GetUninitializedObject(type);
        return Activator.CreateInstance(type);
    }
}