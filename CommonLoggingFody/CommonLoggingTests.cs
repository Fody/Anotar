using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Logging;
using NUnit.Framework;

[TestFixture]
public class CommonLoggingTests
{
    string beforeAssemblyPath;
    Assembly assembly;
    string afterAssemblyPath;
    ActionAdapter actionAdapter;

    public CommonLoggingTests()
    {
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\CommonLoggingAssemblyToProcess\bin\Debug\CommonLoggingAssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
        actionAdapter = new ActionAdapter
                         {
                         };
        LogManager.Adapter = actionAdapter;
    }

    [SetUp]
    public void Setup()
    {
       actionAdapter.Fatals.Clear();
       actionAdapter.Errors.Clear();
       actionAdapter.Debugs.Clear();
       actionAdapter.Infos.Clear();
       actionAdapter.Warns.Clear();
       actionAdapter.Traces.Clear();
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof (string));
        var instance = (dynamic)Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = actionAdapter.Debugs.First();
        Assert.IsTrue(message.Format.StartsWith("Method: 'Void Debug()'. Line: ~"));
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'void Debug()'. Line: ~"));
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'Void Debug()'. Line: ~"));
    }

    void CheckException(Action<object> action, List<LogEvent> list, string expected)
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
        Assert.IsTrue(first.Format.StartsWith(expected), first.Format);
    }

    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, actionAdapter.Debugs, expected);
    }


    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, actionAdapter.Debugs, expected);
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, actionAdapter.Infos, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, actionAdapter.Infos, expected);
    }

    [Test]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, actionAdapter.Warns, expected);
    }

    [Test]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, actionAdapter.Warns, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, actionAdapter.Errors, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, actionAdapter.Errors, expected);
    }

    [Test]
    public void OnExceptionToFatal()
    {
		var expected = "Exception occurred in 'void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, actionAdapter.Fatals, expected);
    }

    [Test]
	public void OnExceptionToFatalWithReturn()
    {
		var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
		Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, actionAdapter.Fatals, expected);
    }


    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'void DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'void DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'void DebugStringException()'. Line: ~"));
    }
    [Test]
    public void Trace()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Trace();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        Assert.IsTrue(actionAdapter.Traces.First().Format.StartsWith("Method: 'void Trace()'. Line: ~"));
    }

    [Test]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceString();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        Assert.IsTrue(actionAdapter.Traces.First().Format.StartsWith("Method: 'void TraceString()'. Line: ~"));
    }

    [Test]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        Assert.IsTrue(actionAdapter.Traces.First().Format.StartsWith("Method: 'void TraceStringParams()'. Line: ~"));
    }

    [Test]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        Assert.IsTrue(actionAdapter.Traces.First().Format.StartsWith("Method: 'void TraceStringException()'. Line: ~"));
    }
    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Info();
        Assert.AreEqual(1, actionAdapter.Infos.Count);
        Assert.IsTrue(actionAdapter.Infos.First().Format.StartsWith("Method: 'void Info()'. Line: ~"));
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoString();
        Assert.AreEqual(1, actionAdapter.Infos.Count);
        Assert.IsTrue(actionAdapter.Infos.First().Format.StartsWith("Method: 'void InfoString()'. Line: ~"));
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.AreEqual(1, actionAdapter.Infos.Count);
        Assert.IsTrue(actionAdapter.Infos.First().Format.StartsWith("Method: 'void InfoStringParams()'. Line: ~"));
    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.AreEqual(1, actionAdapter.Infos.Count);
        Assert.IsTrue(actionAdapter.Infos.First().Format.StartsWith("Method: 'void InfoStringException()'. Line: ~"));
    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warn();
        Assert.AreEqual(1, actionAdapter.Warns.Count);
        Assert.IsTrue(actionAdapter.Warns.First().Format.StartsWith("Method: 'void Warn()'. Line: ~"));
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnString();
        Assert.AreEqual(1, actionAdapter.Warns.Count);
        Assert.IsTrue(actionAdapter.Warns.First().Format.StartsWith("Method: 'void WarnString()'. Line: ~"));
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.AreEqual(1, actionAdapter.Warns.Count);
        Assert.IsTrue(actionAdapter.Warns.First().Format.StartsWith("Method: 'void WarnStringParams()'. Line: ~"));
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.AreEqual(1, actionAdapter.Warns.Count);
        Assert.IsTrue(actionAdapter.Warns.First().Format.StartsWith("Method: 'void WarnStringException()'. Line: ~"));
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        Assert.IsTrue(actionAdapter.Errors.First().Format.StartsWith("Method: 'void Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        Assert.IsTrue(actionAdapter.Errors.First().Format.StartsWith("Method: 'void ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        Assert.IsTrue(actionAdapter.Errors.First().Format.StartsWith("Method: 'void ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        Assert.IsTrue(actionAdapter.Errors.First().Format.StartsWith("Method: 'void ErrorStringException()'. Line: ~"));
    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
		instance.FatalString();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        Assert.IsTrue(actionAdapter.Fatals.First().Format.StartsWith("Method: 'void FatalString()'. Line: ~"));
    }

    [Test]
	public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
		instance.FatalStringParams();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        Assert.IsTrue(actionAdapter.Fatals.First().Format.StartsWith("Method: 'void FatalStringParams()'. Line: ~"));
    }

    [Test]
	public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
		instance.FatalStringException();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        Assert.IsTrue(actionAdapter.Fatals.First().Format.StartsWith("Method: 'void FatalStringException()'. Line: ~"));
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
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'Void AsyncMethod()'. Line: ~"));
    }
    [Test]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        ((IEnumerable<int>)instance.EnumeratorMethod()).ToList();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), actionAdapter.Debugs.First().Format);
    }
    [Test]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DelegateMethod();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), actionAdapter.Debugs.First().Format);
    }
    [Test]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.LambdaMethod();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), actionAdapter.Debugs.First().Format);
    }

}