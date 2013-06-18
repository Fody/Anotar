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
        Assert.AreEqual("System.Void GenericClass`1::Debug()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.AreEqual("9", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::Debug()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WithOldLogging()
    {
        var type = assembly.GetType("ClassWithOldLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.AreEqual("7", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithOldLogging::Debug()", logEvent.Value("MethodName"));
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
        Assert.AreEqual("System.Void ClassWithExistingField::Debug()", logEvent.Value("MethodName"));
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
        var expected = "Exception occurred in 'System.Void OnException::ToDebug(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, debugs, expected);
    }

    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToDebugWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, debugs, expected);
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToInfo(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, infos, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToInfoWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
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
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToWarnWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
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
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToErrorWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, errors, expected);
    }

    [Test]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToFatal(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, fatals, expected);
    }

    [Test]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToFatalWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
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
        Assert.AreEqual("13", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::DebugString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }
    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        var logEvent = debugs.Single();
        Assert.AreEqual("17", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::DebugStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        var logEvent = debugs.Single();
        Assert.AreEqual("21", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::DebugStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Info();
        var logEvent = infos.Single();
        Assert.AreEqual("25", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::Info()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        var logEvent = infos.Single();
        Assert.AreEqual("29", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::InfoString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        var logEvent = infos.Single();
        Assert.AreEqual("33", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::InfoStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        var logEvent = infos.Single();
        Assert.AreEqual("37", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::InfoStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warn();
        var logEvent = warns.Single();
        Assert.AreEqual("41", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::Warn()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        var logEvent = warns.Single();
        Assert.AreEqual("45", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::WarnString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        var logEvent = warns.Single();
        Assert.AreEqual("49", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::WarnStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        var logEvent = warns.Single();
        Assert.AreEqual("53", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::WarnStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Error();
        var logEvent = errors.Single();
        Assert.AreEqual("57", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::Error()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        var logEvent = errors.Single();
        Assert.AreEqual("61", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::ErrorString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        var logEvent = errors.Single();
        Assert.AreEqual("65", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::ErrorStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        var logEvent = errors.Single();
        Assert.AreEqual("69", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::ErrorStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Fatal();
        var logEvent = fatals.Single();
        Assert.AreEqual("73", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::Fatal()", logEvent.Value("MethodName"));
        Assert.AreEqual("", logEvent.MessageTemplate.Text);

    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        var logEvent = fatals.Single();
        Assert.AreEqual("77", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::FatalString()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        var logEvent = fatals.Single();
        Assert.AreEqual("81", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::FatalStringParams()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage {0}", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        var logEvent = fatals.Single();
        Assert.AreEqual("85", logEvent.Value("LineNumber"));
        Assert.AreEqual("System.Void ClassWithLogging::FatalStringException()", logEvent.Value("MethodName"));
        Assert.AreEqual("TheMessage", logEvent.MessageTemplate.Text);
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
    }
}