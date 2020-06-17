using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fody;
using NLog;
using NLog.Config;
using Xunit;

public class NLogTests
{
    static Assembly assembly;
    public static List<string> Errors = new List<string>();
    public static List<string> Fatals = new List<string>();
    public static List<string> Debugs = new List<string>();
    public static List<string> Traces = new List<string>();
    public static List<string> Informations = new List<string>();
    public static List<string> Warns = new List<string>();

    static NLogTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: new[] { "0x80131869" }).Assembly;
        var config = new LoggingConfiguration();
        var target = new ActionTarget
        {
            Action = LogEvent
        };

        config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, target));
        config.AddTarget("debugger", target);
        LogManager.Configuration = config;
    }

    public NLogTests()
    {
        Fatals.Clear();
        Errors.Clear();
        Traces.Clear();
        Debugs.Clear();
        Informations.Clear();
        Warns.Clear();
    }

    static void LogEvent(LogEventInfo eventInfo)
    {
        if (eventInfo.Level == LogLevel.Fatal)
        {
            Fatals.Add(eventInfo.FormattedMessage);
            return;
        }

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
            Informations.Add(eventInfo.FormattedMessage);
            return;
        }

        if (eventInfo.Level == LogLevel.Debug)
        {
            Debugs.Add(eventInfo.FormattedMessage);
            return;
        }

        if (eventInfo.Level == LogLevel.Trace)
        {
            Traces.Add(eventInfo.FormattedMessage);
// ReSharper disable RedundantJumpStatement
            return;
// ReSharper restore RedundantJumpStatement
        }
    }

    [Fact]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.Single(Errors);
        Assert.StartsWith("Method: 'Void Method()'. Line: ~", Errors.First());
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
        var message = Debugs.First();
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", message);
    }


    [Fact]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.Single(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static));
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", Debugs.First());
    }

    // ReSharper disable once UnusedParameter.Local
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

        Assert.NotNull(exception);
        Assert.Single(list);
        var first = list.First();
        Assert.True(first.StartsWith(expected), first);
    }


    [Fact]
    public void OnExceptionToTrace()
    {
        var expected = "Exception occurred in 'Void ToTrace(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        CheckException(action, Traces, expected);
    }

    [Fact]
    public void OnExceptionToTraceWithReturn()
    {
        var expected = "Exception occurred in 'Object ToTraceWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        CheckException(action, Traces, expected);
    }

    [Fact]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Fact]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, Debugs, expected);
    }

    [Fact]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, Informations, expected);
    }

    [Fact]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, Informations, expected);
    }

    [Fact]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, Warns, expected);
    }

    [Fact]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, Warns, expected);
    }

    [Fact]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, Errors, expected);
    }

    [Fact]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, Errors, expected);
    }

    [Fact]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, Fatals, expected);
    }

    [Fact]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, Fatals, expected);
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
        Assert.Single(Traces);
        Assert.StartsWith("Method: 'Void Trace()'. Line: ~", Traces.First());
    }

    [Fact]
    public void TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        Assert.Single(Traces);
        Assert.StartsWith("Method: 'Void TraceString()'. Line: ~", Traces.First());
    }

    [Fact]
    public void TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        Assert.Single(Traces);
        Assert.StartsWith("Method: 'Void TraceStringFunc()'. Line: ~", Traces.First());
    }

    [Fact]
    public void TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        Assert.Single(Traces);
        Assert.StartsWith("Method: 'Void TraceStringParams()'. Line: ~", Traces.First());
    }

    [Fact]
    public void TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        Assert.Single(Traces);
        Assert.StartsWith("Method: 'Void TraceStringException()'. Line: ~", Traces.First());
    }

    [Fact]
    public void TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        Assert.Single(Traces);
        Assert.StartsWith("Method: 'Void TraceStringExceptionFunc()'. Line: ~", Traces.First());
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
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Void Debug()'. Line: ~", Debugs.First());
    }

    [Fact]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Void DebugString()'. Line: ~", Debugs.First());
    }

    [Fact]
    public void DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Void DebugStringFunc()'. Line: ~", Debugs.First());
    }

    [Fact]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Void DebugStringParams()'. Line: ~", Debugs.First());
    }

    [Fact]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Void DebugStringException()'. Line: ~", Debugs.First());
    }

    [Fact]
    public void DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~", Debugs.First());
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
        Assert.Single(Informations);
        Assert.StartsWith("Method: 'Void Info()'. Line: ~", Informations.First());
    }

    [Fact]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        Assert.Single(Informations);
        Assert.StartsWith("Method: 'Void InfoString()'. Line: ~", Informations.First());
    }

    [Fact]
    public void InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        Assert.Single(Informations);
        Assert.StartsWith("Method: 'Void InfoStringFunc()'. Line: ~", Informations.First());
    }

    [Fact]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.Single(Informations);
        Assert.StartsWith("Method: 'Void InfoStringParams()'. Line: ~", Informations.First());
    }

    [Fact]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        Assert.Single(Informations);
        Assert.StartsWith("Method: 'Void InfoStringException()'. Line: ~", Informations.First());
    }

    [Fact]
    public void InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        Assert.Single(Informations);
        Assert.StartsWith("Method: 'Void InfoStringExceptionFunc()'. Line: ~", Informations.First());
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
        Assert.Single(Warns);
        Assert.StartsWith("Method: 'Void Warn()'. Line: ~", Warns.First());
    }

    [Fact]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        Assert.Single(Warns);
        Assert.StartsWith("Method: 'Void WarnString()'. Line: ~", Warns.First());
    }

    [Fact]
    public void WarnStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringFunc();
        Assert.Single(Warns);
        Assert.StartsWith("Method: 'Void WarnStringFunc()'. Line: ~", Warns.First());
    }

    [Fact]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.Single(Warns);
        Assert.StartsWith("Method: 'Void WarnStringParams()'. Line: ~", Warns.First());
    }

    [Fact]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        Assert.Single(Warns);
        Assert.StartsWith("Method: 'Void WarnStringException()'. Line: ~", Warns.First());
    }

    [Fact]
    public void WarnStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionFunc();
        Assert.Single(Warns);
        Assert.StartsWith("Method: 'Void WarnStringExceptionFunc()'. Line: ~", Warns.First());
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
        Assert.Single(Errors);
        Assert.StartsWith("Method: 'Void Error()'. Line: ~", Errors.First());
    }

    [Fact]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.Single(Errors);
        Assert.StartsWith("Method: 'Void ErrorString()'. Line: ~", Errors.First());
    }

    [Fact]
    public void ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.Single(Errors);
        Assert.StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~", Errors.First());
    }

    [Fact]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.Single(Errors);
        Assert.StartsWith("Method: 'Void ErrorStringParams()'. Line: ~", Errors.First());
    }

    [Fact]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        Assert.Single(Errors);
        Assert.StartsWith("Method: 'Void ErrorStringException()'. Line: ~", Errors.First());
    }

    [Fact]
    public void ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        Assert.Single(Errors);
        Assert.StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~", Errors.First());
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
        Assert.Single(Fatals);
        Assert.StartsWith("Method: 'Void Fatal()'. Line: ~", Fatals.First());
    }

    [Fact]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        Assert.Single(Fatals);
        Assert.StartsWith("Method: 'Void FatalString()'. Line: ~", Fatals.First());
    }

    [Fact]
    public void FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        Assert.Single(Fatals);
        Assert.StartsWith("Method: 'Void FatalStringFunc()'. Line: ~", Fatals.First());
    }

    [Fact]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.Single(Fatals);
        Assert.StartsWith("Method: 'Void FatalStringParams()'. Line: ~", Fatals.First());
    }

    [Fact]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        Assert.Single(Fatals);
        Assert.StartsWith("Method: 'Void FatalStringException()'. Line: ~", Fatals.First());
    }

    [Fact]
    public void FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        Assert.Single(Fatals);
        Assert.StartsWith("Method: 'Void FatalStringExceptionFunc()'. Line: ~", Fatals.First());
    }

    [Fact]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        Assert.Single(Debugs);
        Assert.StartsWith("Method: 'Task AsyncMethod()'. Line: ~", Debugs.First());
    }

    [Fact]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        Assert.Single(Debugs);
        Assert.True(Debugs.First().StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), Debugs.First());
    }

    [Fact]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        Assert.Single(Debugs);
        Assert.True(Debugs.First().StartsWith("Method: 'Void DelegateMethod()'. Line: ~"), Debugs.First());
    }

    [Fact]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        Assert.Single(Debugs);
        Assert.True(Debugs.First().StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), Debugs.First());
    }

    [Fact]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        Assert.Single(Debugs);
        Assert.True(Debugs.First().StartsWith("Method: 'Void LambdaMethod()'. Line: ~"), Debugs.First());
    }
}