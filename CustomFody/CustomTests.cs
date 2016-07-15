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
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = LoggerFactory.DebugEntries.First();
        Assert.IsTrue(message.Format.StartsWith("Method: 'void Debug()'. Line: ~"));
    }


    [Test]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'void Method()'. Line: ~"));
    }

    [Test]
    public void EnsureLoggerFactoryAttributeisRemoved()
    {
        var first = assembly.GetCustomAttributes(false).FirstOrDefault(x => x.GetType().Name.Contains("LoggerFactoryAttribute"));
        Assert.IsNull(first);
    }

    [Test]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.AreEqual("a", instance.MethodThatReturns("x", 6));
    }

    [Test]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.AreEqual(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Length);
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'void Debug()'. Line: ~"));
    }

    // ReSharper disable once UnusedParameter.Local
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
        Assert.IsTrue(message.StartsWith(expected), message);
    }

    [Test]
    public void OnExceptionToTrace()
    {
        var expected = "Exception occurred in 'void ToTrace(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        CheckException(action, LoggerFactory.TraceEntries, expected);
    }

    [Test]
    public void OnExceptionToTraceWithReturn()
    {
        var expected = "Exception occurred in 'Object ToTraceWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        CheckException(action, LoggerFactory.TraceEntries, expected);
    }

    [Test]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Test]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Test]
    public void OnExceptionToInformation()
    {
        var expected = "Exception occurred in 'void ToInformation(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformation("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Test]
    public void OnExceptionToInformationWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInformationWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformationWithReturn("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Test]
    public void OnExceptionToWarning()
    {
        var expected = "Exception occurred in 'void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Test]
    public void OnExceptionToWarningWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Test]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected);
    }

    [Test]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected);
    }

    [Test]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected);
    }

    [Test]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected);
    }

    [Test]
    public void IsTraceEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsTraceEnabled());
    }

    [Test]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        Assert.AreEqual(1, LoggerFactory.TraceEntries.Count);
        Assert.IsTrue(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'void TraceString()'. Line: ~"));
    }

    [Test]
    public void TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        Assert.AreEqual(1, LoggerFactory.TraceEntries.Count);
        Assert.IsTrue(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'void TraceStringFunc()'. Line: ~"));
    }

    [Test]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.AreEqual(1, LoggerFactory.TraceEntries.Count);
        Assert.IsTrue(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'void TraceStringParams()'. Line: ~"));
    }

    [Test]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.AreEqual(1, LoggerFactory.TraceEntries.Count);
        Assert.IsTrue(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'void TraceStringException()'. Line: ~"));
    }

    [Test]
    public void TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        Assert.AreEqual(1, LoggerFactory.TraceEntries.Count);
        Assert.IsTrue(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'void TraceStringExceptionFunc()'. Line: ~"));
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
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'void Debug()'. Line: ~"));
    }

    [Test]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'void DebugString()'. Line: ~"));
    }

    [Test]
    public void DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'void DebugStringFunc()'. Line: ~"));
    }

    [Test]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'void DebugStringParams()'. Line: ~"));
    }

    [Test]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'void DebugStringException()'. Line: ~"));
    }

    [Test]
    public void DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        Assert.AreEqual(1, LoggerFactory.DebugEntries.Count);
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'void DebugStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void IsInformationEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsInformationEnabled());
    }

    [Test]
    public void Information()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Information();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'void Information()'. Line: ~"));
    }

    [Test]
    public void InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationString();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'void InformationString()'. Line: ~"));
    }

    [Test]
    public void InformationStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringFunc();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'void InformationStringFunc()'. Line: ~"));
    }

    [Test]
    public void InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringParams();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'void InformationStringParams()'. Line: ~"));
    }

    [Test]
    public void InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringException();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'void InformationStringException()'. Line: ~"));
    }

    [Test]
    public void InformationStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringExceptionFunc();
        Assert.AreEqual(1, LoggerFactory.InformationEntries.Count);
        Assert.IsTrue(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'void InformationStringExceptionFunc()'. Line: ~"));
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
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'void Warning()'. Line: ~"));
    }

    [Test]
    public void WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'void WarningString()'. Line: ~"));
    }

    [Test]
    public void WarningStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringFunc();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'void WarningStringFunc()'. Line: ~"));
    }

    [Test]
    public void WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'void WarningStringParams()'. Line: ~"));
    }

    [Test]
    public void WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'void WarningStringException()'. Line: ~"));
    }

    [Test]
    public void WarningStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringExceptionFunc();
        Assert.AreEqual(1, LoggerFactory.WarningEntries.Count);
        Assert.IsTrue(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'void WarningStringExceptionFunc()'. Line: ~"));
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
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'void Error()'. Line: ~"));
    }

    [Test]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'void ErrorString()'. Line: ~"));
    }

    [Test]
    public void ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'void ErrorStringFunc()'. Line: ~"));
    }

    [Test]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'void ErrorStringParams()'. Line: ~"));
    }

    [Test]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'void ErrorStringException()'. Line: ~"));
    }

    [Test]
    public void ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        Assert.AreEqual(1, LoggerFactory.ErrorEntries.Count);
        Assert.IsTrue(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'void ErrorStringExceptionFunc()'. Line: ~"));
    }

    [Test]
    public void IsFatalEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.IsTrue(instance.IsFatalEnabled());
    }

    [Test]
    public void Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Fatal();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'void Fatal()'. Line: ~"));
    }

    [Test]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'void FatalString()'. Line: ~"));
    }

    [Test]
    public void FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'void FatalStringFunc()'. Line: ~"));
    }

    [Test]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'void FatalStringParams()'. Line: ~"));
    }

    [Test]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'void FatalStringException()'. Line: ~"));
    }

    [Test]
    public void FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        Assert.AreEqual(1, LoggerFactory.FatalEntries.Count);
        Assert.IsTrue(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'void FatalStringExceptionFunc()'. Line: ~"));
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
        Assert.IsTrue(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void AsyncMethod()'. Line: ~"));
    }

    [Test]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.IsTrue(message.StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), message);
    }

    [Test]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.IsTrue(message.StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), message);
    }

    [Test]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.IsTrue(message.StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), message);
    }

    [Test]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.IsTrue(message.StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), message);
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