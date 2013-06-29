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
    List<LogEvent> infos;
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
            infos.Add(eventInfo);
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
        infos = new List<LogEvent>();
        warns = new List<LogEvent>();
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof (string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.AreEqual("7", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void Debug()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.AreEqual("8", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void Debug()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WithStaticConstructor()
    {
        var type = assembly.GetType("ClassWithStaticConstructor");
        type.GetMethod("StaticMethod", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
        var message = (string) type.GetField("Message", BindingFlags.Static | BindingFlags.Public).GetValue(null);
        Assert.AreEqual("Foo", message);
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, debugs.Count);
        var logEvent = debugs.First();
        Assert.AreEqual("16", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void Debug()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
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
        CheckException(action, infos, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, infos, expected);
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
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        var logEvent = debugs.Single();
        Assert.AreEqual("12", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void DebugString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        var logEvent = debugs.Single();
        Assert.AreEqual("16", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void DebugStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        var logEvent = debugs.Single();
        Assert.AreEqual("20", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void DebugStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Info();
        var logEvent = infos.Single();
        Assert.AreEqual("24", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void Info()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        var logEvent = infos.Single();
        Assert.AreEqual("28", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void InfoString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        var logEvent = infos.Single();
        Assert.AreEqual("32", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void InfoStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        var logEvent = infos.Single();
        Assert.AreEqual("36", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void InfoStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warn();
        var logEvent = warns.Single();
        Assert.AreEqual("40", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void Warn()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        var logEvent = warns.Single();
        Assert.AreEqual("44", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void WarnString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        var logEvent = warns.Single();
        Assert.AreEqual("48", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void WarnStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        var logEvent = warns.Single();
        Assert.AreEqual("52", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void WarnStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Error();
        var logEvent = errors.Single();
        Assert.AreEqual("56", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void Error()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        var logEvent = errors.Single();
        Assert.AreEqual("60", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void ErrorString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        var logEvent = errors.Single();
        Assert.AreEqual("64", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void ErrorStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        var logEvent = errors.Single();
        Assert.AreEqual("68", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void ErrorStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Fatal();
        var logEvent = fatals.Single();
        Assert.AreEqual("72", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void Fatal()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        var logEvent = fatals.Single();
        Assert.AreEqual("76", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void FatalString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        var logEvent = fatals.Single();
        Assert.AreEqual("80", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void FatalStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        var logEvent = fatals.Single();
        Assert.AreEqual("84", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void FatalStringException()", logEvent.Value("MethodName"));
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
        Assert.AreEqual("10", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void AsyncMethod()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        ((IEnumerable<int>)instance.EnumeratorMethod()).ToList();
        var logEvent = debugs.Single();
        Assert.AreEqual("16", logEvent.Value("LineNumber"));
        Assert.AreEqual("IEnumerable<Int32> EnumeratorMethod()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DelegateMethod();
        var logEvent = debugs.Single();
        Assert.AreEqual("22", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void DelegateMethod()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.LambdaMethod();
        var logEvent = debugs.Single();
        Assert.AreEqual("29", logEvent.Value("LineNumber"));
        Assert.AreEqual("Void LambdaMethod()", logEvent.Value("MethodName"));
        Assert.AreEqual("Foo {0}", logEvent.MessageTemplate.Text);
    }
}