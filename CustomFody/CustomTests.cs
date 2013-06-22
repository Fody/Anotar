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
        var message = LoggerFactory.DebugEntries.First();
        Assert.IsTrue(message.Format.StartsWith("Method: 'System.Void GenericClass`1::Debug()'. Line: ~"));
    }

    [Test]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Debug()'. Line: ~"));
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Count());
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'System.Void ClassWithExistingField::Debug()'. Line: ~"));
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
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToDebug(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToDebugWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Test]
    public void OnExceptionToInformation()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToInformation(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformation("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Test]
    public void OnExceptionToInformationWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToInformationWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformationWithReturn("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Test]
    public void OnExceptionToWarning()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToWarning(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Test]
    public void OnExceptionToWarningWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToWarningWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToError(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToErrorWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected);
    }
    [Test]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'System.Void OnException::ToFatal(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected);
    }

    [Test]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'System.Object OnException::ToFatalWithReturn(System.String,System.Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected);
    }


    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::DebugStringException()'. Line: ~"));
    }

    [Test]
    public void Information()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Information();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Information()'. Line: ~"));
    }

    [Test]
    public void InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationString();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::InformationString()'. Line: ~"));
    }

    [Test]
    public void InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationStringParams();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::InformationStringParams()'. Line: ~"));
    }

    [Test]
    public void InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.InformationStringException();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::InformationStringException()'. Line: ~"));
    }

    [Test]
    public void Warning()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Warning();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Warning()'. Line: ~"));
    }

    [Test]
    public void WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarningString();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::WarningString()'. Line: ~"));
    }

    [Test]
    public void WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarningStringParams();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::WarningStringParams()'. Line: ~"));
    }

    [Test]
    public void WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.WarningStringException();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::WarningStringException()'. Line: ~"));
    }

    [Test]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Error();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::ErrorStringException()'. Line: ~"));
    }
    
    [Test]
    public void Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.Fatal();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::Fatal()'. Line: ~"));
    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalString();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::FatalString()'. Line: ~"));
    }

    [Test]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::FatalStringParams()'. Line: ~"));
    }

    [Test]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic)Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'System.Void ClassWithLogging::FatalStringException()'. Line: ~"));
    }
    
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
}