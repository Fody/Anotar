using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using NLog.Config;
using NUnit.Framework;


[TestFixture]
public class MinimalOutputTests
{
    string beforeAssemblyPath;
    Assembly assembly;
    public List<string> Errors = new List<string>();
    public List<string> Debugs = new List<string>();
    public List<string> Infos = new List<string>();
    public List<string> Warns = new List<string>();
    string afterAssemblyPath;

    public MinimalOutputTests()
    {
        this.beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugMinimal\MinimalOutputAssemblyToProcess.dll");
#if (!DEBUG)
        this.beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(this.beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);
        var config = new LoggingConfiguration();
        var target = new ActionTarget
            {
                Action = LogEvent
            };

        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
        config.AddTarget("debuger", target);
        LogManager.Configuration = config;
    }

    void LogEvent(LogEventInfo eventInfo)
    {
        if (eventInfo.Level == LogLevel.Error)
        {
            Errors.Add(eventInfo.FormattedMessage);
            return;
        }        
        if (eventInfo.Level == LogLevel.Warn)
        {
            Warns.Add(eventInfo.FormattedMessage);
            return;
        }        
        if (eventInfo.Level == LogLevel.Info)
        {
            Infos.Add(eventInfo.FormattedMessage);
            return;
        }
        if (eventInfo.Level == LogLevel.Debug)
        {
            Debugs.Add(eventInfo.FormattedMessage);
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
        Assert.AreEqual(string.Empty, Debugs.First());
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.AreEqual(string.Empty, Debugs.First());
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, Debugs.Count);
        Assert.AreEqual(string.Empty, Debugs.First());
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
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, Debugs.Count);
        Assert.AreEqual("TheMessage", Debugs.First());
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, Debugs.Count);
        Assert.AreEqual("TheMessage 1", Debugs.First());

    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, Debugs.Count);
        Assert.AreEqual("TheMessage", Debugs.First());
    }

    [Test]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Info();
        Assert.AreEqual(1, Infos.Count);
        Assert.AreEqual(string.Empty, Infos.First());
    }

    [Test]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoString();
        Assert.AreEqual(1, Infos.Count);
        Assert.AreEqual("TheMessage", Infos.First());
    }

    [Test]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.AreEqual(1, Infos.Count);
        Assert.AreEqual("TheMessage 1", Infos.First());
    }

    [Test]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.AreEqual(1, Infos.Count);
        Assert.AreEqual("TheMessage",Infos.First());
    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warn();
        Assert.AreEqual(1, Warns.Count);
        Assert.AreEqual(string.Empty, Warns.First());
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnString();
        Assert.AreEqual(1, Warns.Count);
        Assert.AreEqual("TheMessage", Warns.First());
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.AreEqual(1, Warns.Count);
        Assert.AreEqual("TheMessage 1", Warns.First());
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.AreEqual(1, Warns.Count);
        Assert.AreEqual("TheMessage", Warns.First());
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, Errors.Count);
        Assert.AreEqual(string.Empty, Errors.First());
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, Errors.Count);
        Assert.AreEqual(string.Empty, Errors.First());
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, Errors.Count);
        Assert.AreEqual("TheMessage 1", Errors.First());
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, Errors.Count);
        Assert.AreEqual("TheMessage", Errors.First());
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
}