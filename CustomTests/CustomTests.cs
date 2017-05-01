using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

public class CustomTests : IDisposable
{
    static object weaverLock = new object();

    string beforeAssemblyPath;
    static IDictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
    string afterAssemblyPath;

    private Assembly WeaveAssembly(string target)
    {
        lock (weaverLock)
        {
            if (assemblies.ContainsKey(target) == false)
            {
                AppDomainAssemblyFinder.Attach();
                var assemblyPathUri = new Uri(new Uri(typeof(CustomTests).GetTypeInfo().Assembly.CodeBase), $"../../../../CustomAssemblyToProcess/bin/Debug/{target}/CustomAssemblyToProcess.dll");
                beforeAssemblyPath = Path.GetFullPath(assemblyPathUri.LocalPath);
#if (!DEBUG)
                beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif
                afterAssemblyPath = WeaverHelper.Weave(beforeAssemblyPath, target);
                assemblies[target] = Assembly.LoadFile(afterAssemblyPath);
            }

            return assemblies[target];
        }
    }

    public void Dispose()
    {
        LoggerFactory.Clear();
    }

    [Theory, MemberData(nameof(Targets))]
    public void Generic(string target)
    {
        var type = WeaveAssembly(target).GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = LoggerFactory.DebugEntries.First();
        Assert.True(message.Format.StartsWith("Method: 'Void Debug()'. Line: ~"));
    }


    [Theory, MemberData(nameof(Targets))]
    public void ClassWithComplexExpressionInLog(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.Equal(1, LoggerFactory.ErrorEntries.Count);
        Assert.True(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'Void Method()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void EnsureLoggerFactoryAttributeisRemoved(string target)
    {
        var first = WeaveAssembly(target).GetCustomAttributes(false).FirstOrDefault(x => x.GetType().Name.Contains("LoggerFactoryAttribute"));
        Assert.Null(first);
    }

    [Theory, MemberData(nameof(Targets))]
    public void MethodThatReturns(string target)
    {
        var type = WeaveAssembly(target).GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.Equal("a", instance.MethodThatReturns("x", 6));
    }

    [Theory, MemberData(nameof(Targets))]
    public void ClassWithExistingField(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithExistingField");
        Assert.Equal(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Length);
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Equal(1, LoggerFactory.DebugEntries.Count);
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void Debug()'. Line: ~"));
    }

    // ReSharper disable once UnusedParameter.Local
    void CheckException(Action<object> action, List<LogEntry> list, string expected, string target)
    {
        Exception exception = null;
        var type = WeaveAssembly(target).GetType("OnException");
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
        Assert.Equal(1, list.Count);
        var first = list.First();
        var message = first.Format;
        Assert.True(message.StartsWith(expected), message);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToTrace(string target)
    {
        var expected = "Exception occurred in 'Void ToTrace(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        CheckException(action, LoggerFactory.TraceEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToTraceWithReturn(string target)
    {
        var expected = "Exception occurred in 'Object ToTraceWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        CheckException(action, LoggerFactory.TraceEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToDebug(string target)
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToDebugWithReturn(string target)
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, LoggerFactory.DebugEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToInformation(string target)
    {
        var expected = "Exception occurred in 'Void ToInformation(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformation("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToInformationWithReturn(string target)
    {
        var expected = "Exception occurred in 'Object ToInformationWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformationWithReturn("x", 6);
        CheckException(action, LoggerFactory.InformationEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToWarning(string target)
    {
        var expected = "Exception occurred in 'Void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToWarningWithReturn(string target)
    {
        var expected = "Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        CheckException(action, LoggerFactory.WarningEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToError(string target)
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToErrorWithReturn(string target)
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, LoggerFactory.ErrorEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToFatal(string target)
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToFatalWithReturn(string target)
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, LoggerFactory.FatalEntries, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsTraceEnabled(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsTraceEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void TraceString(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        Assert.Equal(1, LoggerFactory.TraceEntries.Count);
        Assert.True(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'Void TraceString()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void TraceStringFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        Assert.Equal(1, LoggerFactory.TraceEntries.Count);
        Assert.True(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'Void TraceStringFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void TraceStringParams(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.Equal(1, LoggerFactory.TraceEntries.Count);
        Assert.True(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'Void TraceStringParams()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void TraceStringException(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.Equal(1, LoggerFactory.TraceEntries.Count);
        Assert.True(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'Void TraceStringException()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void TraceStringExceptionFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        Assert.Equal(1, LoggerFactory.TraceEntries.Count);
        Assert.True(LoggerFactory.TraceEntries.First().Format.StartsWith("Method: 'Void TraceStringExceptionFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsDebugEnabled(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsDebugEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Debug(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Equal(1, LoggerFactory.DebugEntries.Count);
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void Debug()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugString(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.Equal(1, LoggerFactory.DebugEntries.Count);
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void DebugString()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugStringFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.Equal(1, LoggerFactory.DebugEntries.Count);
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void DebugStringFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugStringParams(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.Equal(1, LoggerFactory.DebugEntries.Count);
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void DebugStringParams()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugStringException(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.Equal(1, LoggerFactory.DebugEntries.Count);
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void DebugStringException()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugStringExceptionFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        Assert.Equal(1, LoggerFactory.DebugEntries.Count);
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsInformationEnabled(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsInformationEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Information(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Information();
        Assert.Equal(1, LoggerFactory.InformationEntries.Count);
        Assert.True(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'Void Information()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationString(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationString();
        Assert.Equal(1, LoggerFactory.InformationEntries.Count);
        Assert.True(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'Void InformationString()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationStringFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringFunc();
        Assert.Equal(1, LoggerFactory.InformationEntries.Count);
        Assert.True(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'Void InformationStringFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationStringParams(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringParams();
        Assert.Equal(1, LoggerFactory.InformationEntries.Count);
        Assert.True(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'Void InformationStringParams()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationStringException(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringException();
        Assert.Equal(1, LoggerFactory.InformationEntries.Count);
        Assert.True(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'Void InformationStringException()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationStringExceptionFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringExceptionFunc();
        Assert.Equal(1, LoggerFactory.InformationEntries.Count);
        Assert.True(LoggerFactory.InformationEntries.First().Format.StartsWith("Method: 'Void InformationStringExceptionFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsWarningEnabled(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsWarningEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Warning(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warning();
        Assert.Equal(1, LoggerFactory.WarningEntries.Count);
        Assert.True(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'Void Warning()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningString(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        Assert.Equal(1, LoggerFactory.WarningEntries.Count);
        Assert.True(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'Void WarningString()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningStringFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringFunc();
        Assert.Equal(1, LoggerFactory.WarningEntries.Count);
        Assert.True(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'Void WarningStringFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningStringParams(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        Assert.Equal(1, LoggerFactory.WarningEntries.Count);
        Assert.True(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'Void WarningStringParams()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningStringException(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        Assert.Equal(1, LoggerFactory.WarningEntries.Count);
        Assert.True(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'Void WarningStringException()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningStringExceptionFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringExceptionFunc();
        Assert.Equal(1, LoggerFactory.WarningEntries.Count);
        Assert.True(LoggerFactory.WarningEntries.First().Format.StartsWith("Method: 'Void WarningStringExceptionFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsErrorEnabled(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsErrorEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Error(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Error();
        Assert.Equal(1, LoggerFactory.ErrorEntries.Count);
        Assert.True(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'Void Error()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorString(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.Equal(1, LoggerFactory.ErrorEntries.Count);
        Assert.True(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'Void ErrorString()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorStringFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.Equal(1, LoggerFactory.ErrorEntries.Count);
        Assert.True(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorStringParams(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.Equal(1, LoggerFactory.ErrorEntries.Count);
        Assert.True(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'Void ErrorStringParams()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorStringException(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.Equal(1, LoggerFactory.ErrorEntries.Count);
        Assert.True(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'Void ErrorStringException()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorStringExceptionFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        Assert.Equal(1, LoggerFactory.ErrorEntries.Count);
        Assert.True(LoggerFactory.ErrorEntries.First().Format.StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsFatalEnabled(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsFatalEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Fatal(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Fatal();
        Assert.Equal(1, LoggerFactory.FatalEntries.Count);
        Assert.True(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'Void Fatal()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalString(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        Assert.Equal(1, LoggerFactory.FatalEntries.Count);
        Assert.True(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'Void FatalString()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalStringFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        Assert.Equal(1, LoggerFactory.FatalEntries.Count);
        Assert.True(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'Void FatalStringFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalStringParams(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.Equal(1, LoggerFactory.FatalEntries.Count);
        Assert.True(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'Void FatalStringParams()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalStringException(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.Equal(1, LoggerFactory.FatalEntries.Count);
        Assert.True(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'Void FatalStringException()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalStringExceptionFunc(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        Assert.Equal(1, LoggerFactory.FatalEntries.Count);
        Assert.True(LoggerFactory.FatalEntries.First().Format.StartsWith("Method: 'Void FatalStringExceptionFunc()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void PeVerify(string target)
    {
        WeaveAssembly(target);
        Verifier.Verify(beforeAssemblyPath, afterAssemblyPath);
    }

    [Theory, MemberData(nameof(Targets))]
    public void AsyncMethod(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncMethod();
        Assert.True(LoggerFactory.DebugEntries.First().Format.StartsWith("Method: 'Void AsyncMethod()'. Line: ~"));
    }

    [Theory, MemberData(nameof(Targets))]
    public void EnumeratorMethod(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), message);
    }

    [Theory, MemberData(nameof(Targets))]
    public void DelegateMethod(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), message);
    }

    [Theory, MemberData(nameof(Targets))]
    public void AsyncDelegateMethod(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), message);
    }

    [Theory, MemberData(nameof(Targets))]
    public void LambdaMethod(string target)
    {
        var type = WeaveAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        Assert.True(message.StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), message);
    }

    [Fact(Skip= "need to fix ref")]
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

    public static IEnumerable<object[]> Targets
    {
        get
        {
            yield return new object[]{ "sl5" };
            yield return new object[]{ "net462" };
            yield return new object[]{ "netcoreapp1.1" };
        }
    }
}