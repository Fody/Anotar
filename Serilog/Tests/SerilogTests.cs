using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fody;
using Serilog;
using Serilog.Events;
using Xunit;

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

        errors = new List<LogEvent>();
        fatals = new List<LogEvent>();
        debugs = new List<LogEvent>();
        verboses = new List<LogEvent>();
        informations = new List<LogEvent>();
        warns = new List<LogEvent>();
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: new[] { "0x80131869" }).Assembly;
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
        errors = new List<LogEvent>();
        fatals = new List<LogEvent>();
        debugs = new List<LogEvent>();
        verboses = new List<LogEvent>();
        informations = new List<LogEvent>();
        warns = new List<LogEvent>();
    }

    [Fact]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.Single(errors);
        var text = errors.First().MessageTemplate.Text;
        Assert.Equal("X", text);
    }

    [Fact(Skip = "Todo")]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.Equal(7, logEvent.LineNumber());
        Assert.Equal("Void Debug()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.True(logEvent.SourceContext().StartsWith("GenericClass`1"), logEvent.SourceContext());
    }

    [Fact]
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.Equal("a", instance.MethodThatReturns("x", 6));
    }

    [Fact]
    public void WithStaticConstructor()
    {
        var type = assembly.GetType("ClassWithStaticConstructor");
        var flags = BindingFlags.Static | BindingFlags.Public;
        type.GetMethod("StaticMethod", flags).Invoke(null, null);
        // ReSharper disable once PossibleNullReferenceException
        var message = (string) type.GetField("Message", flags).GetValue(null);
        Assert.Equal("Foo", message);
    }

    [Fact(Skip = "Todo")]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.Single(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static));
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Single(debugs);
        var logEvent = debugs.First();
        Assert.Equal(17, logEvent.LineNumber());
        Assert.Equal("Void Debug()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithExistingField", logEvent.SourceContext());
    }

    // ReSharper disable once UnusedParameter.Local
    void CheckException(Action<object> action, List<LogEvent> list, string expected)
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.Throws<Exception>(() =>
        {
            action(instance);
        });
        Assert.Single(list);
        var first = list.First();
        Assert.True(first.MessageTemplate.Text.StartsWith(expected), first.MessageTemplate.Text);
    }

    [Fact]
    public void OnExceptionToVerbose()
    {
        var expected = "Exception occurred in 'Void ToVerbose(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToVerbose("x", 6);
        CheckException(action, verboses, expected);
    }

    [Fact]
    public void OnExceptionToVerboseWithReturn()
    {
        var expected = "Exception occurred in 'Object ToVerboseWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToVerboseWithReturn("x", 6);
        CheckException(action, verboses, expected);
    }

    [Fact]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, debugs, expected);
    }

    [Fact]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, debugs, expected);
    }

    [Fact]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, informations, expected);
    }

    [Fact]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, informations, expected);
    }

    [Fact]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, warns, expected);
    }

    [Fact]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, warns, expected);
    }

    [Fact]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, errors, expected);
    }

    [Fact]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, errors, expected);
    }

    [Fact]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, fatals, expected);
    }

    [Fact]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, fatals, expected);
    }

    [Fact]
    public void IsVerboseEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsVerboseEnabled());
    }

    [Fact]
    public void Verbose()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Verbose();
        var logEvent = verboses.Single();
        Assert.Equal(13, logEvent.LineNumber());
        Assert.Equal("Void Verbose()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void VerboseString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseString();
        var logEvent = verboses.Single();
        Assert.Equal(18, logEvent.LineNumber());
        Assert.Equal("Void VerboseString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void VerboseStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseStringParams();
        var logEvent = verboses.Single();
        Assert.Equal(23, logEvent.LineNumber());
        Assert.Equal("Void VerboseStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void VerboseStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseStringException();
        var logEvent = verboses.Single();
        Assert.Equal(28, logEvent.LineNumber());
        Assert.Equal("Void VerboseStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
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
        var logEvent = debugs.Single();
        Assert.Equal(37, logEvent.LineNumber());
        Assert.Equal("Void Debug()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        var logEvent = debugs.Single();
        Assert.Equal(42, logEvent.LineNumber());
        Assert.Equal("Void DebugString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        var logEvent = debugs.Single();
        Assert.Equal(47, logEvent.LineNumber());
        Assert.Equal("Void DebugStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        var logEvent = debugs.Single();
        Assert.Equal(52, logEvent.LineNumber());
        Assert.Equal("Void DebugStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
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
        var logEvent = informations.Single();
        Assert.Equal(62, logEvent.LineNumber());
        Assert.Equal("Void Information()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationString();
        var logEvent = informations.Single();
        Assert.Equal(67, logEvent.LineNumber());
        Assert.Equal("Void InformationString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringParams();
        var logEvent = informations.Single();
        Assert.Equal(72, logEvent.LineNumber());
        Assert.Equal("Void InformationStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringException();
        var logEvent = informations.Single();
        Assert.Equal(77, logEvent.LineNumber());
        Assert.Equal("Void InformationStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
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
        var logEvent = warns.Single();
        Assert.Equal(87, logEvent.LineNumber());
        Assert.Equal("Void Warning()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        var logEvent = warns.Single();
        Assert.Equal(92, logEvent.LineNumber());
        Assert.Equal("Void WarningString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        var logEvent = warns.Single();
        Assert.Equal(97, logEvent.LineNumber());
        Assert.Equal("Void WarningStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        var logEvent = warns.Single();
        Assert.Equal(102, logEvent.LineNumber());
        Assert.Equal("Void WarningStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
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
        var logEvent = errors.Single();
        Assert.Equal(112, logEvent.LineNumber());
        Assert.Equal("Void Error()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        var logEvent = errors.Single();
        Assert.Equal(117, logEvent.LineNumber());
        Assert.Equal("Void ErrorString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        var logEvent = errors.Single();
        Assert.Equal(122, logEvent.LineNumber());
        Assert.Equal("Void ErrorStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        var logEvent = errors.Single();
        Assert.Equal(127, logEvent.LineNumber());
        Assert.Equal("Void ErrorStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
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
        var logEvent = fatals.Single();
        Assert.Equal(137, logEvent.LineNumber());
        Assert.Equal("Void Fatal()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        var logEvent = fatals.Single();
        Assert.Equal(142, logEvent.LineNumber());
        Assert.Equal("Void FatalString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        var logEvent = fatals.Single();
        Assert.Equal(147, logEvent.LineNumber());
        Assert.Equal("Void FatalStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        var logEvent = fatals.Single();
        Assert.Equal(152, logEvent.LineNumber());
        Assert.Equal("Void FatalStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Fact(Skip = "Todo")]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        var logEvent = debugs.Single();
        Assert.Equal(11, logEvent.LineNumber());
        Assert.Equal("Task AsyncMethod()", logEvent.MethodName());
        Assert.Equal("Foo", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Fact]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        var logEvent = debugs.Single();
        Assert.Equal(18, logEvent.LineNumber());
        Assert.Equal("IEnumerable<Int32> EnumeratorMethod()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Fact]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        var logEvent = debugs.Single();
        Assert.Equal(25, logEvent.LineNumber());
        Assert.Equal("Void DelegateMethod()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Fact]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var logEvent = debugs.Single();
        Assert.Equal(40, logEvent.LineNumber());
        Assert.Equal("Void AsyncDelegateMethod()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Fact]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        var logEvent = debugs.Single();
        Assert.Equal(32, logEvent.LineNumber());
        Assert.Equal("Void LambdaMethod()", logEvent.MethodName());
        Assert.Equal("Foo {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }
}