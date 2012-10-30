using System;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public abstract class BaseTests
{
    Assembly assembly;

    protected BaseTests(string assemblyPath)
    {
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif
        assembly  = WeaverHelper.Weave(assemblyPath);
    }


    [Test]
    public void ClassWithLogging()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }
#endif

}