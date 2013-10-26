using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Catel.Logging;
using NUnit.Framework;

[TestFixture]
public class CatelTests
{
    string beforeAssemblyPath;
    Assembly assembly;
    public List<string> Errors = new List<string>();
    public List<string> Debugs = new List<string>();
    public List<string> Infos = new List<string>();
    public List<string> Warnings = new List<string>();
    string afterAssemblyPath;

    public CatelTests()
    {
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\CatelAssemblyToProcess\bin\Debug\CatelAssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
        LogManager.AddListener(new LogListener
                               {
                                   Action =LogMessage
                               });
    }

    void LogMessage(string message, LogEvent logEvent)
    {
        if (logEvent == LogEvent.Error)
        {
            Errors.Add(message);
            return;
        }
        if (logEvent == LogEvent.Warning)
        {
            Warnings.Add(message);
            return;
        }
        if (logEvent == LogEvent.Info)
        {
            Infos.Add(message);
            return;
        }
        if (logEvent == LogEvent.Debug)
        {
            Debugs.Add(message);
// ReSharper disable once RedundantJumpStatement
            return;
        }        

    }

    [SetUp]
    public void Setup()
    {
        Errors.Clear();
        Debugs.Clear();
        Infos.Clear();
        Warnings.Clear();
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof (string));
        var instance = (dynamic)Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = Debugs.First();
        Assert.IsTrue(message.Split(']').Last().StartsWith(" Method: 'Void Debug()'. Line: ~"), message);
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithLogging] Method: 'void Debug()'. Line: ~"));
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithExistingField] Method: 'void Debug()'. Line: ~"));
    }

    void CheckException(Action<object> action, List<string> list, string expected)
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
        var first = list.First();
        Assert.IsTrue(first.StartsWith(expected),first);
    }


    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "[OnException] Exception occurred in 'void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "[OnException] Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "[OnException] Exception occurred in 'void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, Infos, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "[OnException] Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, Infos, expected);
    }

    [Test]
    public void OnExceptionToWarning()
    {
        var expected = "[OnException] Exception occurred in 'void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        CheckException(action, Warnings, expected);
    }

    [Test]
    public void OnExceptionToWarningWithReturn()
    {
        var expected = "[OnException] Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        CheckException(action, Warnings, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "[OnException] Exception occurred in 'void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, Errors, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "[OnException] Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, Errors, expected);
    }

    [Test]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic)Activator.CreateInstance(type);
     
        Assert.AreEqual("a",instance.MethodThatReturns("x", 6));
    }


    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithLogging] Method: 'void DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithLogging] Method: 'void DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithLogging] Method: 'void DebugStringException()'. Line: ~"));
    }

    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Info();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("[ClassWithLogging] Method: 'void Info()'. Line: ~"));
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoString();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("[ClassWithLogging] Method: 'void InfoString()'. Line: ~"));
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("[ClassWithLogging] Method: 'void InfoStringParams()'. Line: ~"));
    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("[ClassWithLogging] Method: 'void InfoStringException()'. Line: ~"));
    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warn();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("[ClassWithLogging] Method: 'void Warn()'. Line: ~"));
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnString();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("[ClassWithLogging] Method: 'void WarnString()'. Line: ~"));
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("[ClassWithLogging] Method: 'void WarnStringParams()'. Line: ~"));
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("[ClassWithLogging] Method: 'void WarnStringException()'. Line: ~"));
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("[ClassWithLogging] Method: 'void Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("[ClassWithLogging] Method: 'void ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("[ClassWithLogging] Method: 'void ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("[ClassWithLogging] Method: 'void ErrorStringException()'. Line: ~"));
    }
    
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }



    [Test]
    public void AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.AsyncMethod();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithCompilerGeneratedClasses] Method: 'Void AsyncMethod()'. Line: ~"));
    }
    [Test]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        ((IEnumerable<int>)instance.EnumeratorMethod()).ToList();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithCompilerGeneratedClasses] Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), Debugs.First());
    }
    [Test]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DelegateMethod();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithCompilerGeneratedClasses] Method: 'Void DelegateMethod()'. Line: ~"), Debugs.First());
    }
    [Test]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.LambdaMethod();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("[ClassWithCompilerGeneratedClasses] Method: 'Void LambdaMethod()'. Line: ~"), Debugs.First());
    }
}