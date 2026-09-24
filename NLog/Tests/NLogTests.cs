using System.Reflection;
using Fody;
using NLog;
using NLog.Config;

// tests share static logger state
[NotInParallel]
public class NLogTests
{
    static Assembly assembly;
    public static List<string> Errors = new();
    public static List<string> Fatals = new();
    public static List<string> Debugs = new();
    public static List<string> Traces = new();
    public static List<string> Informations = new();
    public static List<string> Warns = new();

    static NLogTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: ["0x80131869"]).Assembly;
        var config = new LoggingConfiguration();
        var target = new ActionTarget
        {
            Action = LogEvent
        };

        config.LoggingRules.Add(new("*", LogLevel.Trace, target));
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

    [Test]
    public async Task ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        await Assert.That(Errors).HasSingleItem();
        await Assert.That(Errors.First()).StartsWith("Method: 'Void Method()'. Line: ~");
    }

    [Test]
    public async Task MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        await Assert.That((string) instance.MethodThatReturns("x", 6)).IsEqualTo("a");
    }

    [Test]
    public async Task Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = Debugs.First();
        await Assert.That(message).StartsWith("Method: 'Void Debug()'. Line: ~");
    }


    [Test]
    public async Task ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        await Assert.That(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static)).HasSingleItem();
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void Debug()'. Line: ~");
    }

    // ReSharper disable once UnusedParameter.Local
    static async Task CheckException(Action<object> action, List<string> list, string expected)
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

        await Assert.That(exception).IsNotNull();
        await Assert.That(list).HasSingleItem();
        var first = list.First();
        await Assert.That(first).StartsWith(expected);
    }


    [Test]
    public async Task OnExceptionToTrace()
    {
        var expected = "Exception occurred in 'Void ToTrace(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        await CheckException(action, Traces, expected);
    }

    [Test]
    public async Task OnExceptionToTraceWithReturn()
    {
        var expected = "Exception occurred in 'Object ToTraceWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        await CheckException(action, Traces, expected);
    }

    [Test]
    public async Task OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        await CheckException(action, Debugs, expected);
    }

    [Test]
    public async Task OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        await CheckException(action, Debugs, expected);
    }

    [Test]
    public async Task OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        await CheckException(action, Informations, expected);
    }

    [Test]
    public async Task OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        await CheckException(action, Informations, expected);
    }

    [Test]
    public async Task OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        await CheckException(action, Warns, expected);
    }

    [Test]
    public async Task OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        await CheckException(action, Warns, expected);
    }

    [Test]
    public async Task OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        await CheckException(action, Errors, expected);
    }

    [Test]
    public async Task OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        await CheckException(action, Errors, expected);
    }

    [Test]
    public async Task OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        await CheckException(action, Fatals, expected);
    }

    [Test]
    public async Task OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        await CheckException(action, Fatals, expected);
    }


    [Test]
    public async Task IsTraceEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsTraceEnabled()).IsTrue();
    }

    [Test]
    public async Task Trace()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Trace();
        await Assert.That(Traces).HasSingleItem();
        await Assert.That(Traces.First()).StartsWith("Method: 'Void Trace()'. Line: ~");
    }

    [Test]
    public async Task TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        await Assert.That(Traces).HasSingleItem();
        await Assert.That(Traces.First()).StartsWith("Method: 'Void TraceString()'. Line: ~");
    }

    [Test]
    public async Task TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        await Assert.That(Traces).HasSingleItem();
        await Assert.That(Traces.First()).StartsWith("Method: 'Void TraceStringFunc()'. Line: ~");
    }

    [Test]
    public async Task TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        await Assert.That(Traces).HasSingleItem();
        await Assert.That(Traces.First()).StartsWith("Method: 'Void TraceStringParams()'. Line: ~");
    }

    [Test]
    public async Task TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        await Assert.That(Traces).HasSingleItem();
        await Assert.That(Traces.First()).StartsWith("Method: 'Void TraceStringException()'. Line: ~");
    }

    [Test]
    public async Task TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        await Assert.That(Traces).HasSingleItem();
        await Assert.That(Traces.First()).StartsWith("Method: 'Void TraceStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task IsDebugEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsDebugEnabled()).IsTrue();
    }

    [Test]
    public async Task Debug()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void Debug()'. Line: ~");
    }

    [Test]
    public async Task DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void DebugString()'. Line: ~");
    }

    [Test]
    public async Task DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void DebugStringFunc()'. Line: ~");
    }

    [Test]
    public async Task DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void DebugStringParams()'. Line: ~");
    }

    [Test]
    public async Task DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void DebugStringException()'. Line: ~");
    }

    [Test]
    public async Task DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task IsInfoEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsInfoEnabled()).IsTrue();
    }

    [Test]
    public async Task Info()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Info();
        await Assert.That(Informations).HasSingleItem();
        await Assert.That(Informations.First()).StartsWith("Method: 'Void Info()'. Line: ~");
    }

    [Test]
    public async Task InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        await Assert.That(Informations).HasSingleItem();
        await Assert.That(Informations.First()).StartsWith("Method: 'Void InfoString()'. Line: ~");
    }

    [Test]
    public async Task InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        await Assert.That(Informations).HasSingleItem();
        await Assert.That(Informations.First()).StartsWith("Method: 'Void InfoStringFunc()'. Line: ~");
    }

    [Test]
    public async Task InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        await Assert.That(Informations).HasSingleItem();
        await Assert.That(Informations.First()).StartsWith("Method: 'Void InfoStringParams()'. Line: ~");
    }

    [Test]
    public async Task InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        await Assert.That(Informations).HasSingleItem();
        await Assert.That(Informations.First()).StartsWith("Method: 'Void InfoStringException()'. Line: ~");
    }

    [Test]
    public async Task InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        await Assert.That(Informations).HasSingleItem();
        await Assert.That(Informations.First()).StartsWith("Method: 'Void InfoStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task IsWarnEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsWarnEnabled()).IsTrue();
    }

    [Test]
    public async Task Warn()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warn();
        await Assert.That(Warns).HasSingleItem();
        await Assert.That(Warns.First()).StartsWith("Method: 'Void Warn()'. Line: ~");
    }

    [Test]
    public async Task WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        await Assert.That(Warns).HasSingleItem();
        await Assert.That(Warns.First()).StartsWith("Method: 'Void WarnString()'. Line: ~");
    }

    [Test]
    public async Task WarnStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringFunc();
        await Assert.That(Warns).HasSingleItem();
        await Assert.That(Warns.First()).StartsWith("Method: 'Void WarnStringFunc()'. Line: ~");
    }

    [Test]
    public async Task WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        await Assert.That(Warns).HasSingleItem();
        await Assert.That(Warns.First()).StartsWith("Method: 'Void WarnStringParams()'. Line: ~");
    }

    [Test]
    public async Task WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        await Assert.That(Warns).HasSingleItem();
        await Assert.That(Warns.First()).StartsWith("Method: 'Void WarnStringException()'. Line: ~");
    }

    [Test]
    public async Task WarnStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionFunc();
        await Assert.That(Warns).HasSingleItem();
        await Assert.That(Warns.First()).StartsWith("Method: 'Void WarnStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task IsErrorEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsErrorEnabled()).IsTrue();
    }

    [Test]
    public async Task Error()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Error();
        await Assert.That(Errors).HasSingleItem();
        await Assert.That(Errors.First()).StartsWith("Method: 'Void Error()'. Line: ~");
    }

    [Test]
    public async Task ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        await Assert.That(Errors).HasSingleItem();
        await Assert.That(Errors.First()).StartsWith("Method: 'Void ErrorString()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        await Assert.That(Errors).HasSingleItem();
        await Assert.That(Errors.First()).StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        await Assert.That(Errors).HasSingleItem();
        await Assert.That(Errors.First()).StartsWith("Method: 'Void ErrorStringParams()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        await Assert.That(Errors).HasSingleItem();
        await Assert.That(Errors.First()).StartsWith("Method: 'Void ErrorStringException()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        await Assert.That(Errors).HasSingleItem();
        await Assert.That(Errors.First()).StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task IsFatalEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsFatalEnabled()).IsTrue();
    }

    [Test]
    public async Task Fatal()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Fatal();
        await Assert.That(Fatals).HasSingleItem();
        await Assert.That(Fatals.First()).StartsWith("Method: 'Void Fatal()'. Line: ~");
    }

    [Test]
    public async Task FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        await Assert.That(Fatals).HasSingleItem();
        await Assert.That(Fatals.First()).StartsWith("Method: 'Void FatalString()'. Line: ~");
    }

    [Test]
    public async Task FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        await Assert.That(Fatals).HasSingleItem();
        await Assert.That(Fatals.First()).StartsWith("Method: 'Void FatalStringFunc()'. Line: ~");
    }

    [Test]
    public async Task FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        await Assert.That(Fatals).HasSingleItem();
        await Assert.That(Fatals.First()).StartsWith("Method: 'Void FatalStringParams()'. Line: ~");
    }

    [Test]
    public async Task FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        await Assert.That(Fatals).HasSingleItem();
        await Assert.That(Fatals.First()).StartsWith("Method: 'Void FatalStringException()'. Line: ~");
    }

    [Test]
    public async Task FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        await Assert.That(Fatals).HasSingleItem();
        await Assert.That(Fatals.First()).StartsWith("Method: 'Void FatalStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Task AsyncMethod()'. Line: ~");
    }

    [Test]
    public async Task EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~");
    }

    [Test]
    public async Task DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void DelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        await Assert.That(Debugs).HasSingleItem();
        await Assert.That(Debugs.First()).StartsWith("Method: 'Void LambdaMethod()'. Line: ~");
    }
}