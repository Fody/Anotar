using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fody;
using Xunit;

public class CustomTests
{
    static Assembly assembly;

    static CustomTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: new[] {"0x80131869"}).Assembly;
    }

    public CustomTests()
    {
        LoggerFactory.Clear();
    }

    [Fact]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = LoggerFactory.DebugEntries.First();
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", message.Format);
    }


    [Fact]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.Single(LoggerFactory.ErrorEntries);
        Assert.StartsWith("Method: 'Void Method()'. Line: ~", LoggerFactory.ErrorEntries.First().Format);
    }

    [Fact]
    public void EnsureLoggerFactoryAttributeIsRemoved()
    {
        var first = assembly.GetCustomAttributes(false).FirstOrDefault(x => x.GetType().Name.Contains("LoggerFactoryAttribute"));
        Assert.Null(first);
    }

    [Fact]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.Equal("a", instance.MethodThatReturns("x", 6));
    }

    [Fact]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.Single(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static));
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Single(LoggerFactory.DebugEntries);
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
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

        Assert.NotNull(exception);
        Assert.Single(list);
        var first = list.First();
        var message = first.Format;
        Assert.True(message.StartsWith(expected), message);
    }

    [Fact]
    public void OnExceptionToTrace()
    {
        var expected = "Exception occurred in 'Void ToTrace(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        CheckException(action, LoggerFactory.TraceEntries, expected);
    }

    [Fact]
    public void OnExceptionToTraceWithReturn()
    {
        var expected = "Exception occurred in 'Object ToTraceWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        CheckException(action, LoggerFactory.TraceEntries, expected);
    }

    [Fact]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Fact]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Fact]
    public void OnExceptionToInformation()
    {
        var expected = "Exception occurred in 'Void ToInformation(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformation("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Fact]
    public void OnExceptionToInformationWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInformationWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformationWithReturn("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Fact]
    public void OnExceptionToWarning()
    {
        var expected = "Exception occurred in 'Void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Fact]
    public void OnExceptionToWarningWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Fact]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected);
    }

    [Fact]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected);
    }

    [Fact]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected);
    }

    [Fact]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected);
    }

    [Fact]
    public void IsTraceEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsTraceEnabled());
    }

    [Fact]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        Assert.Single(LoggerFactory.TraceEntries);
        Assert.StartsWith("Method: 'Void TraceString()'. Line: ~", LoggerFactory.TraceEntries.First().Format);
    }

    [Fact]
    public void TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        Assert.Single(LoggerFactory.TraceEntries);
        Assert.StartsWith("Method: 'Void TraceStringFunc()'. Line: ~", LoggerFactory.TraceEntries.First().Format);
    }

    [Fact]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.Single(LoggerFactory.TraceEntries);
        Assert.StartsWith("Method: 'Void TraceStringParams()'. Line: ~", LoggerFactory.TraceEntries.First().Format);
    }

    [Fact]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.Single(LoggerFactory.TraceEntries);
        Assert.StartsWith("Method: 'Void TraceStringException()'. Line: ~", LoggerFactory.TraceEntries.First().Format);
    }

    [Fact]
    public void TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        Assert.Single(LoggerFactory.TraceEntries);
        Assert.StartsWith("Method: 'Void TraceStringExceptionFunc()'. Line: ~", LoggerFactory.TraceEntries.First().Format);
    }

    [Fact]
    public void IsDebugEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsDebugEnabled());
    }

    [Fact]
    public void Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Single(LoggerFactory.DebugEntries);
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
    }

    [Fact]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.Single(LoggerFactory.DebugEntries);
        Assert.StartsWith("Method: 'Void DebugString()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
    }

    [Fact]
    public void DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.Single(LoggerFactory.DebugEntries);
        Assert.StartsWith("Method: 'Void DebugStringFunc()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
    }

    [Fact]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.Single(LoggerFactory.DebugEntries);
        Assert.StartsWith("Method: 'Void DebugStringParams()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
    }

    [Fact]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.Single(LoggerFactory.DebugEntries);
        Assert.StartsWith("Method: 'Void DebugStringException()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
    }

    [Fact]
    public void DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        Assert.Single(LoggerFactory.DebugEntries);
        Assert.StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
    }

    [Fact]
    public void IsInformationEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsInformationEnabled());
    }

    [Fact]
    public void Information()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Information();
        Assert.Single(LoggerFactory.InformationEntries);
        Assert.StartsWith("Method: 'Void Information()'. Line: ~", LoggerFactory.InformationEntries.First().Format);
    }

    [Fact]
    public void InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationString();
        Assert.Single(LoggerFactory.InformationEntries);
        Assert.StartsWith("Method: 'Void InformationString()'. Line: ~", LoggerFactory.InformationEntries.First().Format);
    }

    [Fact]
    public void InformationStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringFunc();
        Assert.Single(LoggerFactory.InformationEntries);
        Assert.StartsWith("Method: 'Void InformationStringFunc()'. Line: ~", LoggerFactory.InformationEntries.First().Format);
    }

    [Fact]
    public void InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringParams();
        Assert.Single(LoggerFactory.InformationEntries);
        Assert.StartsWith("Method: 'Void InformationStringParams()'. Line: ~", LoggerFactory.InformationEntries.First().Format);
    }

    [Fact]
    public void InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringException();
        Assert.Single(LoggerFactory.InformationEntries);
        Assert.StartsWith("Method: 'Void InformationStringException()'. Line: ~", LoggerFactory.InformationEntries.First().Format);
    }

    [Fact]
    public void InformationStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringExceptionFunc();
        Assert.Single(LoggerFactory.InformationEntries);
        Assert.StartsWith("Method: 'Void InformationStringExceptionFunc()'. Line: ~", LoggerFactory.InformationEntries.First().Format);
    }

    [Fact]
    public void IsWarningEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsWarningEnabled());
    }

    [Fact]
    public void Warning()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warning();
        Assert.Single(LoggerFactory.WarningEntries);
        Assert.StartsWith("Method: 'Void Warning()'. Line: ~", LoggerFactory.WarningEntries.First().Format);
    }

    [Fact]
    public void WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        Assert.Single(LoggerFactory.WarningEntries);
        Assert.StartsWith("Method: 'Void WarningString()'. Line: ~", LoggerFactory.WarningEntries.First().Format);
    }

    [Fact]
    public void WarningStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringFunc();
        Assert.Single(LoggerFactory.WarningEntries);
        Assert.StartsWith("Method: 'Void WarningStringFunc()'. Line: ~", LoggerFactory.WarningEntries.First().Format);
    }

    [Fact]
    public void WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        Assert.Single(LoggerFactory.WarningEntries);
        Assert.StartsWith("Method: 'Void WarningStringParams()'. Line: ~", LoggerFactory.WarningEntries.First().Format);
    }

    [Fact]
    public void WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        Assert.Single(LoggerFactory.WarningEntries);
        Assert.StartsWith("Method: 'Void WarningStringException()'. Line: ~", LoggerFactory.WarningEntries.First().Format);
    }

    [Fact]
    public void WarningStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringExceptionFunc();
        Assert.Single(LoggerFactory.WarningEntries);
        Assert.StartsWith("Method: 'Void WarningStringExceptionFunc()'. Line: ~", LoggerFactory.WarningEntries.First().Format);
    }

    [Fact]
    public void IsErrorEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsErrorEnabled());
    }

    [Fact]
    public void Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Error();
        Assert.Single(LoggerFactory.ErrorEntries);
        Assert.StartsWith("Method: 'Void Error()'. Line: ~", LoggerFactory.ErrorEntries.First().Format);
    }

    [Fact]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.Single(LoggerFactory.ErrorEntries);
        Assert.StartsWith("Method: 'Void ErrorString()'. Line: ~", LoggerFactory.ErrorEntries.First().Format);
    }

    [Fact]
    public void ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.Single(LoggerFactory.ErrorEntries);
        Assert.StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~", LoggerFactory.ErrorEntries.First().Format);
    }

    [Fact]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.Single(LoggerFactory.ErrorEntries);
        Assert.StartsWith("Method: 'Void ErrorStringParams()'. Line: ~", LoggerFactory.ErrorEntries.First().Format);
    }

    [Fact]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.Single(LoggerFactory.ErrorEntries);
        Assert.StartsWith("Method: 'Void ErrorStringException()'. Line: ~", LoggerFactory.ErrorEntries.First().Format);
    }

    [Fact]
    public void ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        Assert.Single(LoggerFactory.ErrorEntries);
        Assert.StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~", LoggerFactory.ErrorEntries.First().Format);
    }

    [Fact]
    public void IsFatalEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsFatalEnabled());
    }

    [Fact]
    public void Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Fatal();
        Assert.Single(LoggerFactory.FatalEntries);
        Assert.StartsWith("Method: 'Void Fatal()'. Line: ~", LoggerFactory.FatalEntries.First().Format);
    }

    [Fact]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        Assert.Single(LoggerFactory.FatalEntries);
        Assert.StartsWith("Method: 'Void FatalString()'. Line: ~", LoggerFactory.FatalEntries.First().Format);
    }

    [Fact]
    public void FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        Assert.Single(LoggerFactory.FatalEntries);
        Assert.StartsWith("Method: 'Void FatalStringFunc()'. Line: ~", LoggerFactory.FatalEntries.First().Format);
    }

    [Fact]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.Single(LoggerFactory.FatalEntries);
        Assert.StartsWith("Method: 'Void FatalStringParams()'. Line: ~", LoggerFactory.FatalEntries.First().Format);
    }

    [Fact]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.Single(LoggerFactory.FatalEntries);
        Assert.StartsWith("Method: 'Void FatalStringException()'. Line: ~", LoggerFactory.FatalEntries.First().Format);
    }

    [Fact]
    public void FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        Assert.Single(LoggerFactory.FatalEntries);
        Assert.StartsWith("Method: 'Void FatalStringExceptionFunc()'. Line: ~", LoggerFactory.FatalEntries.First().Format);
    }

    [Fact]
    public void AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncMethod();
        Assert.StartsWith("Method: 'Void AsyncMethod()'. Line: ~", LoggerFactory.DebugEntries.First().Format);
    }

    [Fact]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), message);
    }

    [Fact]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), message);
    }

    [Fact]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), message);
    }

    [Fact]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), message);
    }
}