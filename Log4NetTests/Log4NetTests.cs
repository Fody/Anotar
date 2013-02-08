using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;

[TestFixture]
public class Log4NetTests
{
    string beforeAssemblyPath;
    Assembly assembly;
    public List<string> Errors = new List<string>();
    public List<string> Debugs = new List<string>();
    public List<string> Infos = new List<string>();
    public List<string> Warns = new List<string>();
    string afterAssemblyPath;

    public Log4NetTests()
    {

        beforeAssemblyPath =Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugLog4Net\Log4NetAssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
        var hierarchy = (Hierarchy)LogManager.GetRepository();
        hierarchy.Root.RemoveAllAppenders(); /*Remove any other appenders*/

        var target = new ActionAppender
        {
            Action = LogEvent
        };


        BasicConfigurator.Configure(target);
    }
    void LogEvent(LoggingEvent loggingEvent)
    {
        if (loggingEvent.Level == Level.Error)
        {
            Errors.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Warn)
        {
            Warns.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Info)
        {
            Infos.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Debug)
        {
            Debugs.Add(loggingEvent.RenderedMessage);
            return;
        }
        if (loggingEvent.Level == Level.Trace)
        {
            Debugs.Add(loggingEvent.RenderedMessage);
            return;
        }

    }

    [SetUp]
    public void Setup()
    {
        Errors.Clear();
        Debugs.Clear();
        Infos.Clear();
        Warns.Clear();
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof (string));
        var instance = (dynamic)Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = Debugs.First();
        Assert.IsTrue(message.StartsWith("Method: 'System.Void GenericClass`1::Debug()'. Line: ~"));
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::Debug()'. Line: ~"));
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithExistingField::Debug()'. Line: ~"));
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
    public void OnExceptionToTrace()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToTrace(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Test]
    public void OnExceptionToTraceWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToTraceWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        CheckException(action, Debugs, expected);
    }
    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToDebug(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, Debugs, expected);
    }


    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToDebugWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToInfo(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, Infos, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToInfoWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, Infos, expected);
    }

    [Test]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToWarn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, Warns, expected);
    }

    [Test]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToWarnWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, Warns, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToError(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, Errors, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToErrorWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, Errors, expected);
    }

    [Test]
    public void Trace()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Trace();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::Trace()'. Line: ~"));
    }
    [Test]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceString();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::TraceString()'. Line: ~"));
    }

    [Test]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::TraceStringParams()'. Line: ~"));
    }

    [Test]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::TraceStringException()'. Line: ~"));
    }


    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, Debugs.Count);
        Assert.IsTrue(Debugs.First().StartsWith("Method: 'System.Void ClassWithLogging::DebugStringException()'. Line: ~"));
    }

    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Info();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("Method: 'System.Void ClassWithLogging::Info()'. Line: ~"));
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoString();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("Method: 'System.Void ClassWithLogging::InfoString()'. Line: ~"));
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("Method: 'System.Void ClassWithLogging::InfoStringParams()'. Line: ~"));
    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.AreEqual(1, Infos.Count);
        Assert.IsTrue(Infos.First().StartsWith("Method: 'System.Void ClassWithLogging::InfoStringException()'. Line: ~"));
    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warn();
        Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().StartsWith("Method: 'System.Void ClassWithLogging::Warn()'. Line: ~"));
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnString();
        Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().StartsWith("Method: 'System.Void ClassWithLogging::WarnString()'. Line: ~"));
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().StartsWith("Method: 'System.Void ClassWithLogging::WarnStringParams()'. Line: ~"));
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.AreEqual(1, Warns.Count);
        Assert.IsTrue(Warns.First().StartsWith("Method: 'System.Void ClassWithLogging::WarnStringException()'. Line: ~"));
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'System.Void ClassWithLogging::Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'System.Void ClassWithLogging::ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, Errors.Count);
        Assert.IsTrue(Errors.First().StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringException()'. Line: ~"));
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
}