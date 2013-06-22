using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class CustomTests
{
    string beforeAssemblyPath;
    Assembly assembly;
    string afterAssemblyPath;

    public CustomTests()
    {

        AppDomainAssemblyFinder.Attach();
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\CustomAssemblyToProcess\bin\Debug\CustomAssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
        afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath);
        assembly = Assembly.LoadFile(afterAssemblyPath);

    }

    [TearDown]
    public void Clean()
    {
        LoggerFactory.Clear();
    }

    [Test]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof (string));
        var instance = (dynamic)Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = LoggerFactory.Debugs.First();
        Assert.IsTrue(message.Format.StartsWith("Method: 'System.Void GenericClass`1::Debug()'. Line: ~"));
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, LoggerFactory.Debugs.Count);
        Assert.IsTrue(LoggerFactory.Debugs.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Debug()'. Line: ~"));
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, LoggerFactory.Debugs.Count);
        Assert.IsTrue(LoggerFactory.Debugs.First().Format.StartsWith("Method: 'System.Void ClassWithExistingField::Debug()'. Line: ~"));
    }

    void CheckException(Action<object> action, List<LogEntry> list, string expected)
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
        var message = first.Format;
        Assert.IsTrue(message.StartsWith(expected),message);
    }


    [Test]
    public void OnExceptionToTrace()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToTrace(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        CheckException(action, LoggerFactory.Traces, expected);
    }

    [Test]
    public void OnExceptionToTraceWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToTraceWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        CheckException(action, LoggerFactory.Traces, expected);
    }
    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToDebug(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, LoggerFactory.Debugs, expected);
    }

    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToDebugWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, LoggerFactory.Debugs, expected);
    }

    [Test]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToInfo(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, LoggerFactory.Infos, expected);
    }

    [Test]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToInfoWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, LoggerFactory.Infos, expected);
    }

    [Test]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToWarn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, LoggerFactory.Warns, expected);
    }

    [Test]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToWarnWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, LoggerFactory.Warns, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToError(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, LoggerFactory.Errors, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToErrorWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, LoggerFactory.Errors, expected);
    }
    [Test]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToFatal(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, LoggerFactory.Fatals, expected);
    }

    [Test]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToFatalWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, LoggerFactory.Fatals, expected);
    }

    [Test]
    public void Trace()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Trace();
        Assert.AreEqual(1, LoggerFactory.Traces.Count);
        Assert.IsTrue(LoggerFactory.Traces.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Trace()'. Line: ~"));
    }
    [Test]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceString();
        Assert.AreEqual(1, LoggerFactory.Traces.Count);
        Assert.IsTrue(LoggerFactory.Traces.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::TraceString()'. Line: ~"));
    }

    [Test]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.AreEqual(1, LoggerFactory.Traces.Count);
        Assert.IsTrue(LoggerFactory.Traces.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::TraceStringParams()'. Line: ~"));
    }

    [Test]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.AreEqual(1, LoggerFactory.Traces.Count);
        Assert.IsTrue(LoggerFactory.Traces.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::TraceStringException()'. Line: ~"));
    }


    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, LoggerFactory.Debugs.Count);
        Assert.IsTrue(LoggerFactory.Debugs.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, LoggerFactory.Debugs.Count);
        Assert.IsTrue(LoggerFactory.Debugs.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, LoggerFactory.Debugs.Count);
        Assert.IsTrue(LoggerFactory.Debugs.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::DebugStringException()'. Line: ~"));
    }

    [Test]
    public void Information()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Information();
        Assert.AreEqual(1, LoggerFactory.Infos.Count);
        Assert.IsTrue(LoggerFactory.Infos.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Information()'. Line: ~"));
    }

    [Test]
    public void InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationString();
        Assert.AreEqual(1, LoggerFactory.Infos.Count);
        Assert.IsTrue(LoggerFactory.Infos.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::InformationString()'. Line: ~"));
    }

    [Test]
    public void InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationStringParams();
        Assert.AreEqual(1, LoggerFactory.Infos.Count);
        Assert.IsTrue(LoggerFactory.Infos.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::InformationStringParams()'. Line: ~"));
    }

    [Test]
    public void InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationStringException();
        Assert.AreEqual(1, LoggerFactory.Infos.Count);
        Assert.IsTrue(LoggerFactory.Infos.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::InformationStringException()'. Line: ~"));
    }

    [Test]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warn();
        Assert.AreEqual(1, LoggerFactory.Warns.Count);
        Assert.IsTrue(LoggerFactory.Warns.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Warn()'. Line: ~"));
    }

    [Test]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnString();
        Assert.AreEqual(1, LoggerFactory.Warns.Count);
        Assert.IsTrue(LoggerFactory.Warns.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::WarnString()'. Line: ~"));
    }

    [Test]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.AreEqual(1, LoggerFactory.Warns.Count);
        Assert.IsTrue(LoggerFactory.Warns.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::WarnStringParams()'. Line: ~"));
    }

    [Test]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.AreEqual(1, LoggerFactory.Warns.Count);
        Assert.IsTrue(LoggerFactory.Warns.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::WarnStringException()'. Line: ~"));
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, LoggerFactory.Errors.Count);
        Assert.IsTrue(LoggerFactory.Errors.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, LoggerFactory.Errors.Count);
        Assert.IsTrue(LoggerFactory.Errors.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, LoggerFactory.Errors.Count);
        Assert.IsTrue(LoggerFactory.Errors.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, LoggerFactory.Errors.Count);
        Assert.IsTrue(LoggerFactory.Errors.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringException()'. Line: ~"));
    }
    
    [Test]
    public void Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Fatal();
        Assert.AreEqual(1, LoggerFactory.Fatals.Count);
        Assert.IsTrue(LoggerFactory.Fatals.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Fatal()'. Line: ~"));
    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalString();
        Assert.AreEqual(1, LoggerFactory.Fatals.Count);
        Assert.IsTrue(LoggerFactory.Fatals.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::FatalString()'. Line: ~"));
    }

    [Test]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.AreEqual(1, LoggerFactory.Fatals.Count);
        Assert.IsTrue(LoggerFactory.Fatals.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::FatalStringParams()'. Line: ~"));
    }

    [Test]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.AreEqual(1, LoggerFactory.Fatals.Count);
        Assert.IsTrue(LoggerFactory.Fatals.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::FatalStringException()'. Line: ~"));
    }
    
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
}