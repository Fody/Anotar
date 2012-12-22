using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public abstract class BaseTests
{
    Assembly assembly;
    public List<string> errors = new List<string>();
    public List<string> debugs = new List<string>();
    public List<string> infos = new List<string>();
    public List<string> warns = new List<string>();

    protected BaseTests(string assemblyPath)
    {
#if (!DEBUG)
        assemblyPath = assemblyPath.Replace("Debug", "Release");
#endif
        assembly  = WeaverHelper.Weave(assemblyPath);
    }

    [SetUp]
    public void Setup()
    {
        errors.Clear();
        debugs.Clear();
        infos.Clear();
        warns.Clear();
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, debugs.Count);
        Assert.IsTrue(debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::Debug()'. Line: ~"));
    }

    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToDebug(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, debugs, expected);
    }

    void CheckException(Action<dynamic> action, List<string> list, string expected)
    {
        Exception exception = null;
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);
        try
        {
            action(instance);
        }
        catch (Exception e)
        {
            exception = e;
        }
        Assert.IsNotNull(exception);
        Assert.AreEqual(1, list.Count);
        Assert.IsTrue(list.First().StartsWith(expected));
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToInfo(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, infos, expected);
    }

    [Test]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToWarn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, warns, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        
        var expected = "Exception occurred in 'System.Void OnException::ToError(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, errors, expected);
    }

    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, debugs.Count);
        Assert.IsTrue(debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, debugs.Count);
        Assert.IsTrue(debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, debugs.Count);
        Assert.IsTrue(debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::DebugStringException()'. Line: ~"));
    }

    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Info();
        Assert.AreEqual(1, infos.Count);
        Assert.IsTrue(infos.First().StartsWith("Method: 'System.Void ClassWithLogging::Info()'. Line: ~"));
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoString();
        Assert.AreEqual(1, infos.Count);
        Assert.IsTrue(infos.First().StartsWith("Method: 'System.Void ClassWithLogging::InfoString()'. Line: ~"));
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.AreEqual(1, infos.Count);
        Assert.IsTrue(infos.First().StartsWith("Method: 'System.Void ClassWithLogging::InfoStringParams()'. Line: ~"));
    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.AreEqual(1, infos.Count);
        Assert.IsTrue(infos.First().StartsWith("Method: 'System.Void ClassWithLogging::InfoStringException()'. Line: ~"));
    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warn();
        Assert.AreEqual(1, warns.Count);
        Assert.IsTrue(warns.First().StartsWith("Method: 'System.Void ClassWithLogging::Warn()'. Line: ~"));
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnString();
        Assert.AreEqual(1, warns.Count);
        Assert.IsTrue(warns.First().StartsWith("Method: 'System.Void ClassWithLogging::WarnString()'. Line: ~"));
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.AreEqual(1, warns.Count);
        Assert.IsTrue(warns.First().StartsWith("Method: 'System.Void ClassWithLogging::WarnStringParams()'. Line: ~"));
    }


    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.AreEqual(1, warns.Count);
        Assert.IsTrue(warns.First().StartsWith("Method: 'System.Void ClassWithLogging::WarnStringException()'. Line: ~"));
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors.First().StartsWith("Method: 'System.Void ClassWithLogging::Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors.First().StartsWith("Method: 'System.Void ClassWithLogging::ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors.First().StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, errors.Count);
        Assert.IsTrue(errors.First().StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringException()'. Line: ~"));
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }
#endif

}