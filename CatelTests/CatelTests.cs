using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Catel.Logging;
using NUnit.Framework;

[TestFixture]
public abstract class CatelTests
{
    string beforeAssemblyPath;
    Assembly assembly;
    public List<string> Errors = new List<string>();
    public List<string> Debugs = new List<string>();
    public List<string> Informations = new List<string>();
    public List<string> Warnings = new List<string>();
    string afterAssemblyPath;

    protected CatelTests(string assemblyName)
    {
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, $@"..\..\..\{assemblyName}\bin\Debug\CatelAssemblyToProcess.dll"));
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);

        LogManager.AddListener(new LogListener
        {
            Action = LogMessage
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
            Informations.Add(message);
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
        Informations.Clear();
        Warnings.Clear();
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = Debugs.First();
        Assert.IsTrue(message.StartsWith("Method: 'Void Debug()'. Line: ~"), message);
    }

    [Test]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'Void Method()'. Line: ~"));
    }

    [Test]
    public void IsDebugEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsDebugEnabled());
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void Debug()'. Line: ~"));
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Length);
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void Debug()'. Line: ~"));
    }

    // ReSharper disable once UnusedParameter.Local
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
        Assert.IsTrue(first.StartsWith(expected), first);
    }


    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, Informations, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, Informations, expected);
    }

    [Test]
    public void OnExceptionToWarning()
    {
        var expected = "Exception occurred in 'Void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        CheckException(action, Warnings, expected);
    }

    [Test]
    public void OnExceptionToWarningWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        CheckException(action, Warnings, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, Errors, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, Errors, expected);
    }

    [Test]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.AreEqual("a", instance.MethodThatReturns("x", 6));
    }


    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void DebugStringFunc()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void DebugStringException()'. Line: ~"));
    }

    [Test]
    public void DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void IsInfoEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsInfoEnabled());
    }

    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Info();
        Assert.AreEqual(1, Informations.Count);
        Assert.IsTrue(Informations.First().StartsWith("Method: 'Void Info()'. Line: ~"));
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        Assert.AreEqual(1, Informations.Count);
        Assert.IsTrue(Informations.First().StartsWith("Method: 'Void InfoString()'. Line: ~"));
    }

    [Test]
    public void InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        Assert.AreEqual(1, Informations.Count);
        Assert.IsTrue(Informations.First().StartsWith("Method: 'Void InfoStringFunc()'. Line: ~"));
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.AreEqual(1, Informations.Count);
        Assert.IsTrue(Informations.First().StartsWith("Method: 'Void InfoStringParams()'. Line: ~"));
    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.AreEqual(1, Informations.Count);
        Assert.IsTrue(Informations.First().StartsWith("Method: 'Void InfoStringException()'. Line: ~"));
    }

    [Test]
    public void InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        Assert.AreEqual(1, Informations.Count);
        Assert.IsTrue(Informations.First().StartsWith("Method: 'Void InfoStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void IsWarningEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsWarningEnabled());
    }

    [Test]
    public void Warning()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warning();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("Method: 'Void Warning()'. Line: ~"));
    }

    [Test]
    public void WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("Method: 'Void WarningString()'. Line: ~"));
    }

    [Test]
    public void WarningStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringFunc();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("Method: 'Void WarningStringFunc()'. Line: ~"));
    }

    [Test]
    public void WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("Method: 'Void WarningStringParams()'. Line: ~"));
    }

    [Test]
    public void WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("Method: 'Void WarningStringException()'. Line: ~"));
    }

    [Test]
    public void WarningStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringExceptionFunc();
        Assert.AreEqual(1, Warnings.Count);
        Assert.IsTrue(Warnings.First().StartsWith("Method: 'Void WarningStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void IsErrorEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsErrorEnabled());
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'Void Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'Void ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'Void ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'Void ErrorStringException()'. Line: ~"));
    }

    [Test]
    public void ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
    }

    [Test]
    public void AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncMethod();
        Assert.IsTrue(Debugs.Any(x => x.StartsWith("Method: 'Void AsyncMethod()'. Line: ~")));
    }

    [Test]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), Debugs.First());
    }

    [Test]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), Debugs.First());
    }

    [Test]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var message = Debugs.Skip(1).First();
        Assert.IsTrue(message.StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), message);
    }

    [Test]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), Debugs.First());
    }

    [Test]
    [Explicit("need to fix ref")]
    public void Issue33()
    {
        // We need to load a custom assembly because the C# compiler won't generate the IL
        // that caused the issue, but NullGuard does.
        var afterIssue33Path = WeaverHelper.Weave(Path.GetFullPath("NullGuardAnotarBug.dll"));
        var issue33Assembly = Assembly.LoadFile(afterIssue33Path);

        var type = issue33Assembly.GetType("NullGuardAnotarBug");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.NotNull(instance.DoIt());
    }
}