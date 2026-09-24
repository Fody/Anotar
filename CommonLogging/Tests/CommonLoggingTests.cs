using System.Reflection;
using Common.Logging;
using Fody;

// tests share static logger state
[NotInParallel]
public class CommonLoggingTests
{
    static Assembly assembly;
    static ActionAdapter actionAdapter;

    static CommonLoggingTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: ["0x80131869"]).Assembly;
        actionAdapter = new();
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

    [Test]
    public async Task ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        await Assert.That(actionAdapter.Errors.First().Format).StartsWith("Method: 'Void Method()'. Line: ~");
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
        var message = actionAdapter.Debugs.First();
        await Assert.That(message.Format).StartsWith("Method: 'Void Debug()'. Line: ~");
    }


    [Test]
    public async Task ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        await Assert.That(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static)).HasSingleItem();
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        await Assert.That(actionAdapter.Debugs.First().Format).StartsWith("Method: 'Void Debug()'. Line: ~");
    }

    // ReSharper disable once UnusedParameter.Local
    static async Task CheckException(Action<object> action, List<LogEvent> list, string expected)
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
        await Assert.That(first.Format).StartsWith(expected);
    }

    [Test]
    public async Task OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        await CheckException(action, actionAdapter.Debugs, expected);
    }

    [Test]
    public async Task OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        await CheckException(action, actionAdapter.Debugs, expected);
    }

    [Test]
    public async Task OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        await CheckException(action, actionAdapter.Informations, expected);
    }

    [Test]
    public async Task OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        await CheckException(action, actionAdapter.Informations, expected);
    }

    [Test]
    public async Task OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        await CheckException(action, actionAdapter.Warnings, expected);
    }

    [Test]
    public async Task OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        await CheckException(action, actionAdapter.Warnings, expected);
    }

    [Test]
    public async Task OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        await CheckException(action, actionAdapter.Errors, expected);
    }

    [Test]
    public async Task OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        await CheckException(action, actionAdapter.Errors, expected);
    }

    [Test]
    public async Task OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        await CheckException(action, actionAdapter.Fatals, expected);
    }

    [Test]
    public async Task OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        await CheckException(action, actionAdapter.Fatals, expected);
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
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        await Assert.That(actionAdapter.Debugs.First().Format).StartsWith("Method: 'Void Debug()'. Line: ~");
    }

    [Test]
    public async Task DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void DebugString()'. Line: ~");
    }

    [Test]
    public async Task DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void DebugStringFunc()'. Line: ~");
    }

    [Test]
    public async Task DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void DebugStringParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void DebugStringException()'. Line: ~");
    }

    [Test]
    public async Task DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task DebugStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionParams();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void DebugStringExceptionParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
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
        await Assert.That(actionAdapter.Traces).HasSingleItem();
        var logEvent = actionAdapter.Traces.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void Trace()'. Line: ~");
    }

    [Test]
    public async Task TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        await Assert.That(actionAdapter.Traces).HasSingleItem();
        var logEvent = actionAdapter.Traces.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void TraceString()'. Line: ~");
    }

    [Test]
    public async Task TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        await Assert.That(actionAdapter.Traces).HasSingleItem();
        var logEvent = actionAdapter.Traces.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void TraceStringFunc()'. Line: ~");
    }

    [Test]
    public async Task TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        await Assert.That(actionAdapter.Traces).HasSingleItem();
        var logEvent = actionAdapter.Traces.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void TraceStringParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        await Assert.That(actionAdapter.Traces).HasSingleItem();
        var logEvent = actionAdapter.Traces.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void TraceStringException()'. Line: ~");
    }

    [Test]
    public async Task TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        await Assert.That(actionAdapter.Traces).HasSingleItem();
        var logEvent = actionAdapter.Traces.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void TraceStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task TraceStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionParams();
        await Assert.That(actionAdapter.Traces).HasSingleItem();
        var logEvent = actionAdapter.Traces.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void TraceStringExceptionParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
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
        await Assert.That(actionAdapter.Informations).HasSingleItem();
        var logEvent = actionAdapter.Informations.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void Info()'. Line: ~");
    }

    [Test]
    public async Task InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        await Assert.That(actionAdapter.Informations).HasSingleItem();
        var logEvent = actionAdapter.Informations.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void InfoString()'. Line: ~");
    }

    [Test]
    public async Task InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        await Assert.That(actionAdapter.Informations).HasSingleItem();
        var logEvent = actionAdapter.Informations.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void InfoStringFunc()'. Line: ~");
    }

    [Test]
    public async Task InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        await Assert.That(actionAdapter.Informations).HasSingleItem();
        var logEvent = actionAdapter.Informations.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void InfoStringParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        await Assert.That(actionAdapter.Informations).HasSingleItem();
        var logEvent = actionAdapter.Informations.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void InfoStringException()'. Line: ~");
    }

    [Test]
    public async Task InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        await Assert.That(actionAdapter.Informations).HasSingleItem();
        var logEvent = actionAdapter.Informations.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void InfoStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task InfoStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionParams();
        await Assert.That(actionAdapter.Informations).HasSingleItem();
        var logEvent = actionAdapter.Informations.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void InfoStringExceptionParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
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
        await Assert.That(actionAdapter.Warnings).HasSingleItem();
        var logEvent = actionAdapter.Warnings.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void Warn()'. Line: ~");
    }

    [Test]
    public async Task WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        await Assert.That(actionAdapter.Warnings).HasSingleItem();
        var logEvent = actionAdapter.Warnings.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void WarnString()'. Line: ~");
    }

    [Test]
    public async Task WarnStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringFunc();
        await Assert.That(actionAdapter.Warnings).HasSingleItem();
        var logEvent = actionAdapter.Warnings.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void WarnStringFunc()'. Line: ~");
    }

    [Test]
    public async Task WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        await Assert.That(actionAdapter.Warnings).HasSingleItem();
        var logEvent = actionAdapter.Warnings.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void WarnStringParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        await Assert.That(actionAdapter.Warnings).HasSingleItem();
        var logEvent = actionAdapter.Warnings.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void WarnStringException()'. Line: ~");
    }

    [Test]
    public async Task WarnStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionFunc();
        await Assert.That(actionAdapter.Warnings).HasSingleItem();
        var logEvent = actionAdapter.Warnings.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void WarnStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task WarnStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionParams();
        await Assert.That(actionAdapter.Warnings).HasSingleItem();
        var logEvent = actionAdapter.Warnings.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void WarnStringExceptionParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
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
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        var logEvent = actionAdapter.Errors.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void Error()'. Line: ~");
    }

    [Test]
    public async Task ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        var logEvent = actionAdapter.Errors.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void ErrorString()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        var logEvent = actionAdapter.Errors.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        var logEvent = actionAdapter.Errors.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void ErrorStringParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        var logEvent = actionAdapter.Errors.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void ErrorStringException()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        var logEvent = actionAdapter.Errors.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionParams();
        await Assert.That(actionAdapter.Errors).HasSingleItem();
        var logEvent = actionAdapter.Errors.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void ErrorStringExceptionParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task IsFatalEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsFatalEnabled()).IsTrue();
    }

    [Test]
    public async Task FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        await Assert.That(actionAdapter.Fatals).HasSingleItem();
        var logEvent = actionAdapter.Fatals.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void FatalString()'. Line: ~");
    }

    [Test]
    public async Task FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        await Assert.That(actionAdapter.Fatals).HasSingleItem();
        var logEvent = actionAdapter.Fatals.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void FatalStringFunc()'. Line: ~");
    }

    [Test]
    public async Task FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        await Assert.That(actionAdapter.Fatals).HasSingleItem();
        var logEvent = actionAdapter.Fatals.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void FatalStringParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        await Assert.That(actionAdapter.Fatals).HasSingleItem();
        var logEvent = actionAdapter.Fatals.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void FatalStringException()'. Line: ~");
    }

    [Test]
    public async Task FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        await Assert.That(actionAdapter.Fatals).HasSingleItem();
        var logEvent = actionAdapter.Fatals.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void FatalStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task FatalStringExceptionParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionParams();
        await Assert.That(actionAdapter.Fatals).HasSingleItem();
        var logEvent = actionAdapter.Fatals.First();
        await Assert.That(logEvent.Exception).IsNotNull();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void FatalStringExceptionParams()'. Line: ~");
        await Assert.That(logEvent.Args.First()).IsEqualTo(1);
    }

    [Test]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        await Assert.That(actionAdapter.Debugs.First().Format).StartsWith("Method: 'Task AsyncMethod()'. Line: ~");
    }

    [Test]
    public async Task EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~");
    }

    [Test]
    public async Task DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void DelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        await Assert.That(actionAdapter.Debugs).HasSingleItem();
        var logEvent = actionAdapter.Debugs.First();
        await Assert.That(logEvent.Format).StartsWith("Method: 'Void LambdaMethod()'. Line: ~");
    }
}