using System.Reflection;
using Fody;
using Serilog;
using Serilog.Events;

// tests share static logger state
[NotInParallel]
public class SerilogTests:IDisposable
{
    static List<LogEvent> errors;
    static List<LogEvent> fatals;
    static List<LogEvent> debugs;
    static List<LogEvent> verboses;
    static List<LogEvent> informations;
    static List<LogEvent> warns;
    static Assembly assembly;

    static void LogEvent(LogEvent eventInfo)
    {
        if (eventInfo.Level == LogEventLevel.Verbose)
        {
            verboses.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Debug)
        {
            debugs.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Fatal)
        {
            fatals.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Error)
        {
            errors.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Information)
        {
            informations.Add(eventInfo);
        }
        if (eventInfo.Level == LogEventLevel.Warning)
        {
            warns.Add(eventInfo);
        }
    }

    static SerilogTests()
    {
        var eventSink = new EventSink
        {
            Action = LogEvent
        };

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Sink(eventSink)
            .CreateLogger();

        errors = new();
        fatals = new();
        debugs = new();
        verboses = new();
        informations = new();
        warns = new();
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: ["0x80131869"]).Assembly;
    }

    public SerilogTests()
    {
        Clear();
    }

    public void Dispose()
    {
        Clear();
    }

    static void Clear()
    {
        errors = new();
        fatals = new();
        debugs = new();
        verboses = new();
        informations = new();
        warns = new();
    }

    [Test]
    public async Task ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        await Assert.That(errors).HasSingleItem();
        var text = errors.First().MessageTemplate.Text;
        await Assert.That(text).IsEqualTo("X");
    }

    [Test]
    [Skip("Todo")]
    public async Task Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(7);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Debug()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).StartsWith("GenericClass`1");
    }

    [Test]
    public async Task MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        await Assert.That((string) instance.MethodThatReturns("x", 6)).IsEqualTo("a");
    }

    [Test]
    public async Task WithStaticConstructor()
    {
        // the static constructor replaces the global logger, so restore it for other tests
        var logger = Log.Logger;
        string message;
        try
        {
            var type = assembly.GetType("ClassWithStaticConstructor");
            var flags = BindingFlags.Static | BindingFlags.Public;
            type.GetMethod("StaticMethod", flags).Invoke(null, null);
            // ReSharper disable once PossibleNullReferenceException
            message = (string) type.GetField("Message", flags).GetValue(null);
        }
        finally
        {
            Log.Logger = logger;
        }

        await Assert.That(message).IsEqualTo("Foo");
    }

    [Test]
    [Skip("Todo")]
    public async Task ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        await Assert.That(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static)).HasSingleItem();
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        await Assert.That(debugs).HasSingleItem();
        var logEvent = debugs.First();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(17);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Debug()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithExistingField");
    }

    // ReSharper disable once UnusedParameter.Local
    static async Task CheckException(Action<object> action, List<LogEvent> list, string expected)
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.Throws<Exception>(() =>
        {
            action(instance);
        });
        await Assert.That(list).HasSingleItem();
        var first = list.First();
        await Assert.That(first.MessageTemplate.Text).StartsWith(expected);
    }

    [Test]
    public async Task OnExceptionToVerbose()
    {
        var expected = "Exception occurred in 'Void ToVerbose(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToVerbose("x", 6);
        await CheckException(action, verboses, expected);
    }

    [Test]
    public async Task OnExceptionToVerboseWithReturn()
    {
        var expected = "Exception occurred in 'Object ToVerboseWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToVerboseWithReturn("x", 6);
        await CheckException(action, verboses, expected);
    }

    [Test]
    public async Task OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        await CheckException(action, debugs, expected);
    }

    [Test]
    public async Task OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        await CheckException(action, debugs, expected);
    }

    [Test]
    public async Task OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        await CheckException(action, informations, expected);
    }

    [Test]
    public async Task OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        await CheckException(action, informations, expected);
    }

    [Test]
    public async Task OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        await CheckException(action, warns, expected);
    }

    [Test]
    public async Task OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        await CheckException(action, warns, expected);
    }

    [Test]
    public async Task OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        await CheckException(action, errors, expected);
    }

    [Test]
    public async Task OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        await CheckException(action, errors, expected);
    }

    [Test]
    public async Task OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        await CheckException(action, fatals, expected);
    }

    [Test]
    public async Task OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        await CheckException(action, fatals, expected);
    }

    [Test]
    public async Task IsVerboseEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsVerboseEnabled()).IsTrue();
    }

    [Test]
    public async Task Verbose()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Verbose();
        var logEvent = verboses.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(12);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Verbose()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task VerboseString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseString();
        var logEvent = verboses.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(17);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void VerboseString()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task VerboseStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseStringParams();
        var logEvent = verboses.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(22);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void VerboseStringParams()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage {0}");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task VerboseStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseStringException();
        var logEvent = verboses.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(27);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void VerboseStringException()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
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
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(36);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Debug()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(41);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void DebugString()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(46);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void DebugStringParams()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage {0}");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(51);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void DebugStringException()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task IsInformationEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsInformationEnabled()).IsTrue();
    }

    [Test]
    public async Task Information()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Information();
        var logEvent = informations.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(61);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Information()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationString();
        var logEvent = informations.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(66);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void InformationString()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringParams();
        var logEvent = informations.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(71);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void InformationStringParams()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage {0}");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringException();
        var logEvent = informations.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(76);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void InformationStringException()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task IsWarningEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsWarningEnabled()).IsTrue();
    }

    [Test]
    public async Task Warning()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warning();
        var logEvent = warns.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(86);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Warning()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        var logEvent = warns.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(91);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void WarningString()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        var logEvent = warns.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(96);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void WarningStringParams()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage {0}");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        var logEvent = warns.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(101);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void WarningStringException()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
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
        var logEvent = errors.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(111);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Error()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        var logEvent = errors.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(116);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void ErrorString()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        var logEvent = errors.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(121);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void ErrorStringParams()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage {0}");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        var logEvent = errors.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(126);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void ErrorStringException()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
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
        var logEvent = fatals.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(136);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void Fatal()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        var logEvent = fatals.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(141);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void FatalString()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        var logEvent = fatals.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(146);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void FatalStringParams()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage {0}");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    public async Task FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        var logEvent = fatals.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(151);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void FatalStringException()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("TheMessage");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithLogging");
    }

    [Test]
    [Skip("Todo")]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(11);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Task AsyncMethod()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("Foo");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithCompilerGeneratedClasses");
    }

    [Test]
    public async Task EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(15);
        await Assert.That(logEvent.MethodName()).IsEqualTo("IEnumerable<Int32> EnumeratorMethod()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithCompilerGeneratedClasses");
    }

    [Test]
    public async Task DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(22);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void DelegateMethod()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithCompilerGeneratedClasses");
    }

    [Test]
    public async Task AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(37);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void AsyncDelegateMethod()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithCompilerGeneratedClasses");
    }

    [Test]
    public async Task LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        var logEvent = debugs.Single();
        await Assert.That(logEvent.LineNumber()).IsEqualTo(29);
        await Assert.That(logEvent.MethodName()).IsEqualTo("Void LambdaMethod()");
        await Assert.That(logEvent.MessageTemplate.Text).IsEqualTo("Foo {0}");
        await Assert.That(logEvent.SourceContext()).IsEqualTo("ClassWithCompilerGeneratedClasses");
    }
}