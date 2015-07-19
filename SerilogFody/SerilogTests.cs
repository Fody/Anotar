using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Serilog;
using Serilog.Events;

[TestFixture]
public class SerilogTests
{
    string beforeAssemblyPath;
    Assembly assembly;
    List<LogEvent> errors;
    List<LogEvent> fatals;
    List<LogEvent> debugs;
    List<LogEvent> informations;
    List<LogEvent> warns;
    string afterAssemblyPath;

    public SerilogTests()
    {
        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\SerilogAssemblyToProcess\bin\Debug\SerilogAssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    void LogEvent(LogEvent eventInfo)
    {
        if (eventInfo.Level == LogEventLevel.Debug)
        {
            debugs.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Fatal)
        {
            fatals.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Error)
        {
            errors.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Information)
        {
            informations.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Warning)
        {
            warns.Add(eventInfo);
        }

    }

    [SetUp]
    public void Setup()
    {
        var eventSink = new EventSink
        {
            Action = LogEvent
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(eventSink)
            .CreateLogger();

        errors = new List<LogEvent>();
        fatals = new List<LogEvent>();
        debugs = new List<LogEvent>();
        informations = new List<LogEvent>();
        warns = new List<LogEvent>();
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic)Activator.CreateInstance(constructedType);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.AreEqual(7, logEvent.LineNumber());
        Assert.AreEqual("Void Debug()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }


    [Test]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic)Activator.CreateInstance(type);

        Assert.AreEqual("a", instance.MethodThatReturns("x", 6));
    }

    [Test]
    public void WithStaticConstructor()
    {
        var type = assembly.GetType("ClassWithStaticConstructor");
        type.GetMethod("StaticMethod", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
        // ReSharper disable once PossibleNullReferenceException
        var message = (string)type.GetField("Message", BindingFlags.Static | BindingFlags.Public).GetValue(null);
        Assert.AreEqual("Foo", message);
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, debugs.Count);
        var logEvent = debugs.First();
        Assert.AreEqual(17, logEvent.LineNumber());
        Assert.AreEqual("Void Debug()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    // ReSharper disable once UnusedParameter.Local
    void CheckException(Action<object> action, List<LogEvent> list, string expected)
    {
        Exception exception = null;
        var type = assembly.GetType("OnException");
        var instance = (dynamic)Activator.CreateInstance(type);
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
        Assert.IsTrue(first.MessageTemplate.Text.StartsWith(expected), first.MessageTemplate.Text);
    }


    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, debugs, expected);
    }

    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, debugs, expected);
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, informations, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, informations, expected);
    }

    [Test]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, warns, expected);
    }

    [Test]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, warns, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, errors, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, errors, expected);
    }

    [Test]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, fatals, expected);
    }

    [Test]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, fatals, expected);
    }


    [Test]
    public void IsDebugEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsDebugEnabled());
    }
    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.AreEqual(13, logEvent.LineNumber());
        Assert.AreEqual("Void Debug()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        var logEvent = debugs.Single();
        Assert.AreEqual(18, logEvent.LineNumber());
        Assert.AreEqual("Void DebugString()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringParams();
        var logEvent = debugs.Single();
        Assert.AreEqual(23, logEvent.LineNumber());
        Assert.AreEqual("Void DebugStringParams()", logEvent.MethodName());
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        var logEvent = debugs.Single();
        Assert.AreEqual(28, logEvent.LineNumber());
        Assert.AreEqual("Void DebugStringException()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }


    [Test]
    public void IsInformationEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsInformationEnabled());
    }
    [Test]
    public void Information()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Information();
        var logEvent = informations.Single();
        Assert.AreEqual(38, logEvent.LineNumber());
        Assert.AreEqual("Void Information()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationString();
        var logEvent = informations.Single();
        Assert.AreEqual(43, logEvent.LineNumber());
        Assert.AreEqual("Void InformationString()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationStringParams();
        var logEvent = informations.Single();
        Assert.AreEqual(48, logEvent.LineNumber());
        Assert.AreEqual("Void InformationStringParams()", logEvent.MethodName());
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationStringException();
        var logEvent = informations.Single();
        Assert.AreEqual(53, logEvent.LineNumber());
        Assert.AreEqual("Void InformationStringException()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);

    }
    [Test]
    public void IsWarningEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsWarningEnabled());
    }

    [Test]
    public void Warning()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warning();
        var logEvent = warns.Single();
        Assert.AreEqual(63, logEvent.LineNumber());
        Assert.AreEqual("Void Warning()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarningString();
        var logEvent = warns.Single();
        Assert.AreEqual(68, logEvent.LineNumber());
        Assert.AreEqual("Void WarningString()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarningStringParams();
        var logEvent = warns.Single();
        Assert.AreEqual(73, logEvent.LineNumber());
        Assert.AreEqual("Void WarningStringParams()", logEvent.MethodName());
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarningStringException();
        var logEvent = warns.Single();
        Assert.AreEqual(78, logEvent.LineNumber());
        Assert.AreEqual("Void WarningStringException()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void IsErrorEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsErrorEnabled());
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        var logEvent = errors.Single();
        Assert.AreEqual(88, logEvent.LineNumber());
        Assert.AreEqual("Void Error()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        var logEvent = errors.Single();
        Assert.AreEqual(93, logEvent.LineNumber());
        Assert.AreEqual("Void ErrorString()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        var logEvent = errors.Single();
        Assert.AreEqual(98, logEvent.LineNumber());
        Assert.AreEqual("Void ErrorStringParams()", logEvent.MethodName());
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        var logEvent = errors.Single();
        Assert.AreEqual(103, logEvent.LineNumber());
        Assert.AreEqual("Void ErrorStringException()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void IsFatalEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsFatalEnabled());
    }

    [Test]
    public void Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Fatal();
        var logEvent = fatals.Single();
        Assert.AreEqual(113, logEvent.LineNumber());
        Assert.AreEqual("Void Fatal()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalString();
        var logEvent = fatals.Single();
        Assert.AreEqual(118, logEvent.LineNumber());
        Assert.AreEqual("Void FatalString()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalStringParams();
        var logEvent = fatals.Single();
        Assert.AreEqual(123, logEvent.LineNumber());
        Assert.AreEqual("Void FatalStringParams()", logEvent.MethodName());
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalStringException();
        var logEvent = fatals.Single();
        Assert.AreEqual(128, logEvent.LineNumber());
        Assert.AreEqual("Void FatalStringException()", logEvent.MethodName());
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
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
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.AsyncMethod();
        var logEvent = debugs.Single();
        Assert.AreEqual(10, logEvent.LineNumber());
        Assert.AreEqual("Void AsyncMethod()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        ((IEnumerable<int>)instance.EnumeratorMethod()).ToList();
        var logEvent = debugs.Single();
        Assert.AreEqual(16, logEvent.LineNumber());
        Assert.AreEqual("IEnumerable<Int32> EnumeratorMethod()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DelegateMethod();
        var logEvent = debugs.Single();
        Assert.AreEqual(23, logEvent.LineNumber());
        Assert.AreEqual("Void DelegateMethod()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var logEvent = debugs.Single();
        Assert.AreEqual(36, logEvent.LineNumber());
        Assert.AreEqual("Void AsyncDelegateMethod()", logEvent.MethodName());
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.LambdaMethod();
        var logEvent = debugs.Single();
        Assert.AreEqual(30, logEvent.LineNumber());
        Assert.AreEqual("Void LambdaMethod()", logEvent.MethodName());
        Assert.AreEqual("Foo {0}", logEvent.MessageTemplate.Text);
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
        var instance = (dynamic)Activator.CreateInstance(type);

        Assert.NotNull(instance.DoIt());
    }
}