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
#if (PORTABLE)
        var path = @"..\..\..\CommonLoggingAssemblyToProcessPortable\bin\Debug\CommonLoggingAssemblyToProcessPortable.dll";
#else
        var path = @"..\..\..\CommonLoggingAssemblyToProcess\bin\Debug\CommonLoggingAssemblyToProcess.dll";
#endif
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.GetFullPath(path);
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
        actionAdapter = new ActionAdapter();
        LogManager.Adapter = actionAdapter;
    }

    [SetUp]
    public void Setup()
    {
        actionAdapter.Fatals.Clear();
        actionAdapter.Errors.Clear();
        actionAdapter.Debugs.Clear();
        actionAdapter.Informations.Clear();
        actionAdapter.Warnings.Clear();
        actionAdapter.Traces.Clear();
    }


    [Test]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        Assert.IsTrue(actionAdapter.Errors.First().Format.StartsWith("Method: 'void Method()'. Line: ~"));
    }

    [Test]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.AreEqual("a", instance.MethodThatReturns("x", 6));
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = actionAdapter.Debugs.First();
        Assert.IsTrue(message.Format.StartsWith("Method: 'Void Debug()'. Line: ~"));
    }


    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'Void Debug()'. Line: ~"));
    }

    // ReSharper disable once UnusedParameter.Local
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
        CheckException(action, actionAdapter.Informations, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, actionAdapter.Informations, expected);
    }

    [Test]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, actionAdapter.Warnings, expected);
    }

    [Test]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, actionAdapter.Warnings, expected);
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
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'void Debug()'. Line: ~"));
    }

    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void DebugStringFunc()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void DebugStringParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void DebugStringException()'. Line: ~"));
    }

    [Test]
    public void DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void DebugStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void DebugStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionParams();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void DebugStringExceptionParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void IsTraceEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsTraceEnabled());
    }

    [Test]
    public void Trace()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Trace();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        var logEvent = actionAdapter.Traces.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void Trace()'. Line: ~"));
    }

    [Test]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        var logEvent = actionAdapter.Traces.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void TraceString()'. Line: ~"));
    }

    [Test]
    public void TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        var logEvent = actionAdapter.Traces.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void TraceStringFunc()'. Line: ~"));
    }

    [Test]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        var logEvent = actionAdapter.Traces.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void TraceStringParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        var logEvent = actionAdapter.Traces.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void TraceStringException()'. Line: ~"));
    }

    [Test]
    public void TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        var logEvent = actionAdapter.Traces.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void TraceStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void TraceStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionParams();
        Assert.AreEqual(1, actionAdapter.Traces.Count);
        var logEvent = actionAdapter.Traces.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void TraceStringExceptionParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
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
        Assert.AreEqual(1, actionAdapter.Informations.Count);
        var logEvent = actionAdapter.Informations.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void Info()'. Line: ~"));
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        Assert.AreEqual(1, actionAdapter.Informations.Count);
        var logEvent = actionAdapter.Informations.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void InfoString()'. Line: ~"));
    }

    [Test]
    public void InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        Assert.AreEqual(1, actionAdapter.Informations.Count);
        var logEvent = actionAdapter.Informations.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void InfoStringFunc()'. Line: ~"));
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.AreEqual(1, actionAdapter.Informations.Count);
        var logEvent = actionAdapter.Informations.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void InfoStringParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.AreEqual(1, actionAdapter.Informations.Count);
        var logEvent = actionAdapter.Informations.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void InfoStringException()'. Line: ~"));
    }

    [Test]
    public void InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        Assert.AreEqual(1, actionAdapter.Informations.Count);
        var logEvent = actionAdapter.Informations.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void InfoStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void InfoStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionParams();
        Assert.AreEqual(1, actionAdapter.Informations.Count);
        var logEvent = actionAdapter.Informations.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void InfoStringExceptionParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void IsWarnEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsWarnEnabled());
    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warn();
        Assert.AreEqual(1, actionAdapter.Warnings.Count);
        var logEvent = actionAdapter.Warnings.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void Warn()'. Line: ~"));
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        Assert.AreEqual(1, actionAdapter.Warnings.Count);
        var logEvent = actionAdapter.Warnings.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void WarnString()'. Line: ~"));
    }

    [Test]
    public void WarnStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringFunc();
        Assert.AreEqual(1, actionAdapter.Warnings.Count);
        var logEvent = actionAdapter.Warnings.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void WarnStringFunc()'. Line: ~"));
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.AreEqual(1, actionAdapter.Warnings.Count);
        var logEvent = actionAdapter.Warnings.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void WarnStringParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.AreEqual(1, actionAdapter.Warnings.Count);
        var logEvent = actionAdapter.Warnings.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void WarnStringException()'. Line: ~"));
    }

    [Test]
    public void WarnStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionFunc();
        Assert.AreEqual(1, actionAdapter.Warnings.Count);
        var logEvent = actionAdapter.Warnings.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void WarnStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void WarnStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionParams();
        Assert.AreEqual(1, actionAdapter.Warnings.Count);
        var logEvent = actionAdapter.Warnings.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void WarnStringExceptionParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
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
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        var logEvent = actionAdapter.Errors.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        var logEvent = actionAdapter.Errors.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        var logEvent = actionAdapter.Errors.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void ErrorStringFunc()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        var logEvent = actionAdapter.Errors.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void ErrorStringParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        var logEvent = actionAdapter.Errors.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void ErrorStringException()'. Line: ~"));
    }

    [Test]
    public void ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        var logEvent = actionAdapter.Errors.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void ErrorStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void ErrorStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionParams();
        Assert.AreEqual(1, actionAdapter.Errors.Count);
        var logEvent = actionAdapter.Errors.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void ErrorStringExceptionParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void IsFatalEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsFatalEnabled());
    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        var logEvent = actionAdapter.Fatals.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void FatalString()'. Line: ~"));
    }

    [Test]
    public void FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        var logEvent = actionAdapter.Fatals.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void FatalStringFunc()'. Line: ~"));
    }

    [Test]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        var logEvent = actionAdapter.Fatals.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void FatalStringParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
    }

    [Test]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        var logEvent = actionAdapter.Fatals.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void FatalStringException()'. Line: ~"));
    }

    [Test]
    public void FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        var logEvent = actionAdapter.Fatals.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void FatalStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void FatalStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionParams();
        Assert.AreEqual(1, actionAdapter.Fatals.Count);
        var logEvent = actionAdapter.Fatals.First();
        Assert.IsNotNull(logEvent.Exception);
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'void FatalStringExceptionParams()'. Line: ~"));
        Assert.AreEqual(1, logEvent.Args.First());
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
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        Assert.IsTrue(actionAdapter.Debugs.First().Format.StartsWith("Method: 'Void AsyncMethod()'. Line: ~"));
    }

    [Test]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), logEvent.Format);
    }

    [Test]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), logEvent.Format);
    }

    [Test]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), logEvent.Format);
    }

    [Test]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        Assert.AreEqual(1, actionAdapter.Debugs.Count);
        var logEvent = actionAdapter.Debugs.First();
        Assert.IsTrue(logEvent.Format.StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), logEvent.Format);
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