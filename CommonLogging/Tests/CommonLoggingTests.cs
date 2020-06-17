using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Common.Logging;
using Fody;
using Xunit;

public class CommonLoggingTests
{
    static Assembly assembly;
    static ActionAdapter actionAdapter;

    static CommonLoggingTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: new[] { "0x80131869" }).Assembly;
        actionAdapter = new ActionAdapter();
        LogManager.Adapter = actionAdapter;
    }

    public CommonLoggingTests()
    {
        actionAdapter.Fatals.Clear();
        actionAdapter.Errors.Clear();
        actionAdapter.Debugs.Clear();
        actionAdapter.Informations.Clear();
        actionAdapter.Warnings.Clear();
        actionAdapter.Traces.Clear();
    }

    [Fact]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.Single(actionAdapter.Errors);
        Assert.StartsWith("Method: 'Void Method()'. Line: ~", actionAdapter.Errors.First().Format);
    }

    [Fact]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.Equal("a", instance.MethodThatReturns("x", 6));
    }

    [Fact]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = actionAdapter.Debugs.First();
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", message.Format);
    }


    [Fact]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.Single(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static));
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Single(actionAdapter.Debugs);
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", actionAdapter.Debugs.First().Format);
    }

    // ReSharper disable once UnusedParameter.Local
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

        Assert.NotNull(exception);
        Assert.Single(list);
        var first = list.First();
        Assert.True(first.Format.StartsWith(expected), first.Format);
    }

    [Fact]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, actionAdapter.Debugs, expected);
    }

    [Fact]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, actionAdapter.Debugs, expected);
    }

    [Fact]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, actionAdapter.Informations, expected);
    }

    [Fact]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, actionAdapter.Informations, expected);
    }

    [Fact]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, actionAdapter.Warnings, expected);
    }

    [Fact]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, actionAdapter.Warnings, expected);
    }

    [Fact]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, actionAdapter.Errors, expected);
    }

    [Fact]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, actionAdapter.Errors, expected);
    }

    [Fact]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, actionAdapter.Fatals, expected);
    }

    [Fact]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, actionAdapter.Fatals, expected);
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
        Assert.Single(actionAdapter.Debugs);
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", actionAdapter.Debugs.First().Format);
    }

    [Fact]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.StartsWith("Method: 'Void DebugString()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.StartsWith("Method: 'Void DebugStringFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.StartsWith("Method: 'Void DebugStringParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void DebugStringException()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void DebugStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionParams();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void DebugStringExceptionParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void IsTraceEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsTraceEnabled());
    }

    [Fact]
    public void Trace()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Trace();
        Assert.Single(actionAdapter.Traces);
        var logEvent = actionAdapter.Traces.First();
        Assert.StartsWith("Method: 'Void Trace()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        Assert.Single(actionAdapter.Traces);
        var logEvent = actionAdapter.Traces.First();
        Assert.StartsWith("Method: 'Void TraceString()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        Assert.Single(actionAdapter.Traces);
        var logEvent = actionAdapter.Traces.First();
        Assert.StartsWith("Method: 'Void TraceStringFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.Single(actionAdapter.Traces);
        var logEvent = actionAdapter.Traces.First();
        Assert.StartsWith("Method: 'Void TraceStringParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.Single(actionAdapter.Traces);
        var logEvent = actionAdapter.Traces.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void TraceStringException()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        Assert.Single(actionAdapter.Traces);
        var logEvent = actionAdapter.Traces.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void TraceStringExceptionFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void TraceStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionParams();
        Assert.Single(actionAdapter.Traces);
        var logEvent = actionAdapter.Traces.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void TraceStringExceptionParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void IsInfoEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsInfoEnabled());
    }

    [Fact]
    public void Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Info();
        Assert.Single(actionAdapter.Informations);
        var logEvent = actionAdapter.Informations.First();
        Assert.StartsWith("Method: 'Void Info()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        Assert.Single(actionAdapter.Informations);
        var logEvent = actionAdapter.Informations.First();
        Assert.StartsWith("Method: 'Void InfoString()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        Assert.Single(actionAdapter.Informations);
        var logEvent = actionAdapter.Informations.First();
        Assert.StartsWith("Method: 'Void InfoStringFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.Single(actionAdapter.Informations);
        var logEvent = actionAdapter.Informations.First();
        Assert.StartsWith("Method: 'Void InfoStringParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.Single(actionAdapter.Informations);
        var logEvent = actionAdapter.Informations.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void InfoStringException()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        Assert.Single(actionAdapter.Informations);
        var logEvent = actionAdapter.Informations.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void InfoStringExceptionFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void InfoStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionParams();
        Assert.Single(actionAdapter.Informations);
        var logEvent = actionAdapter.Informations.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void InfoStringExceptionParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void IsWarnEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsWarnEnabled());
    }

    [Fact]
    public void Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warn();
        Assert.Single(actionAdapter.Warnings);
        var logEvent = actionAdapter.Warnings.First();
        Assert.StartsWith("Method: 'Void Warn()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        Assert.Single(actionAdapter.Warnings);
        var logEvent = actionAdapter.Warnings.First();
        Assert.StartsWith("Method: 'Void WarnString()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void WarnStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringFunc();
        Assert.Single(actionAdapter.Warnings);
        var logEvent = actionAdapter.Warnings.First();
        Assert.StartsWith("Method: 'Void WarnStringFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.Single(actionAdapter.Warnings);
        var logEvent = actionAdapter.Warnings.First();
        Assert.StartsWith("Method: 'Void WarnStringParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.Single(actionAdapter.Warnings);
        var logEvent = actionAdapter.Warnings.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void WarnStringException()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void WarnStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionFunc();
        Assert.Single(actionAdapter.Warnings);
        var logEvent = actionAdapter.Warnings.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void WarnStringExceptionFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void WarnStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionParams();
        Assert.Single(actionAdapter.Warnings);
        var logEvent = actionAdapter.Warnings.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void WarnStringExceptionParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
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
        Assert.Single(actionAdapter.Errors);
        var logEvent = actionAdapter.Errors.First();
        Assert.StartsWith("Method: 'Void Error()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.Single(actionAdapter.Errors);
        var logEvent = actionAdapter.Errors.First();
        Assert.StartsWith("Method: 'Void ErrorString()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.Single(actionAdapter.Errors);
        var logEvent = actionAdapter.Errors.First();
        Assert.StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.Single(actionAdapter.Errors);
        var logEvent = actionAdapter.Errors.First();
        Assert.StartsWith("Method: 'Void ErrorStringParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.Single(actionAdapter.Errors);
        var logEvent = actionAdapter.Errors.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void ErrorStringException()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        Assert.Single(actionAdapter.Errors);
        var logEvent = actionAdapter.Errors.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void ErrorStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionParams();
        Assert.Single(actionAdapter.Errors);
        var logEvent = actionAdapter.Errors.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void ErrorStringExceptionParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void IsFatalEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsFatalEnabled());
    }

    [Fact]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        Assert.Single(actionAdapter.Fatals);
        var logEvent = actionAdapter.Fatals.First();
        Assert.StartsWith("Method: 'Void FatalString()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        Assert.Single(actionAdapter.Fatals);
        var logEvent = actionAdapter.Fatals.First();
        Assert.StartsWith("Method: 'Void FatalStringFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.Single(actionAdapter.Fatals);
        var logEvent = actionAdapter.Fatals.First();
        Assert.StartsWith("Method: 'Void FatalStringParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.Single(actionAdapter.Fatals);
        var logEvent = actionAdapter.Fatals.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void FatalStringException()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        Assert.Single(actionAdapter.Fatals);
        var logEvent = actionAdapter.Fatals.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void FatalStringExceptionFunc()'. Line: ~", logEvent.Format);
    }

    [Fact]
    public void FatalStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionParams();
        Assert.Single(actionAdapter.Fatals);
        var logEvent = actionAdapter.Fatals.First();
        Assert.NotNull(logEvent.Exception);
        Assert.StartsWith("Method: 'Void FatalStringExceptionParams()'. Line: ~", logEvent.Format);
        Assert.Equal(1, logEvent.Args.First());
    }

    [Fact]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        Assert.Single(actionAdapter.Debugs);
        Assert.StartsWith("Method: 'Task AsyncMethod()'. Line: ~", actionAdapter.Debugs.First().Format);
    }

    [Fact]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.True(logEvent.Format.StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), logEvent.Format);
    }

    [Fact]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.True(logEvent.Format.StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), logEvent.Format);
    }

    [Fact]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.True(logEvent.Format.StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), logEvent.Format);
    }

    [Fact]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        Assert.Single(actionAdapter.Debugs);
        var logEvent = actionAdapter.Debugs.First();
        Assert.True(logEvent.Format.StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), logEvent.Format);
    }
}