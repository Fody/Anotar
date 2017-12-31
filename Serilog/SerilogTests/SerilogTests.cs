using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Serilog;
using Serilog.Events;
using Xunit;

public class SerilogTests
{
    static TestAssemblies assemblies = new TestAssemblies("SerilogAssemblyToProcess");
    static List<LogEvent> errors;
    static List<LogEvent> fatals;
    static List<LogEvent> debugs;
    static List<LogEvent> verboses;
    static List<LogEvent> informations;
    static List<LogEvent> warns;

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

    public SerilogTests()
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
    }

    [Theory, MemberData(nameof(Targets))]
    public void ClassWithComplexExpressionInLog(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.Equal(1, errors.Count);
        var text = errors.First().MessageTemplate.Text;
        Assert.Equal(text, "X");
    }

    [Theory, MemberData(nameof(Targets))]
    public void Generic(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.Equal(7, logEvent.LineNumber());
        Assert.Equal("Void Debug()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.True(logEvent.SourceContext().StartsWith("GenericClass`1"), logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void MethodThatReturns(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.Equal("a", instance.MethodThatReturns("x", 6));
    }

    [Theory, MemberData(nameof(Targets))]
    public void WithStaticConstructor(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithStaticConstructor");
        type.GetMethod("StaticMethod", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
        // ReSharper disable once PossibleNullReferenceException
        var message = (string) type.GetField("Message", BindingFlags.Static | BindingFlags.Public).GetValue(null);
        Assert.Equal("Foo", message);
    }

    [Theory, MemberData(nameof(Targets))]
    public void ClassWithExistingField(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithExistingField");
        Assert.Equal(1, type.GetFields(BindingFlags.NonPublic | BindingFlags.Static).Length);
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Equal(1, debugs.Count);
        var logEvent = debugs.First();
        Assert.Equal(17, logEvent.LineNumber());
        Assert.Equal("Void Debug()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithExistingField", logEvent.SourceContext());
    }

    // ReSharper disable once UnusedParameter.Local
    void CheckException(Action<object> action, List<LogEvent> list, string expected, string target)
    {
        var type = assemblies.GetAssembly(target).GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.Throws<Exception>(() =>
        {
            action(instance);
        });
        Assert.Equal(1, list.Count);
        var first = list.First();
        Assert.True(first.MessageTemplate.Text.StartsWith(expected), first.MessageTemplate.Text);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToVerbose(string target)
    {
        var expected = "Exception occurred in 'Void ToVerbose(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToVerbose("x", 6);
        CheckException(action, verboses, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToVerboseWithReturn(string target)
    {
        var expected = "Exception occurred in 'Object ToVerboseWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToVerboseWithReturn("x", 6);
        CheckException(action, verboses, expected, target);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToDebug(string TODO)
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, debugs, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToDebugWithReturn(string TODO)
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, debugs, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToInfo(string TODO)
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, informations, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToInfoWithReturn(string TODO)
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, informations, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToWarn(string TODO)
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, warns, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToWarnWithReturn(string TODO)
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, warns, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToError(string TODO)
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, errors, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToErrorWithReturn(string TODO)
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, errors, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToFatal(string TODO)
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, fatals, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void OnExceptionToFatalWithReturn(string TODO)
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, fatals, expected, TODO);
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsVerboseEnabled(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsVerboseEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Verbose(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Verbose();
        var logEvent = verboses.Single();
        Assert.Equal(13, logEvent.LineNumber());
        Assert.Equal("Void Verbose()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void VerboseString(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseString();
        var logEvent = verboses.Single();
        Assert.Equal(18, logEvent.LineNumber());
        Assert.Equal("Void VerboseString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void VerboseStringParams(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseStringParams();
        var logEvent = verboses.Single();
        Assert.Equal(23, logEvent.LineNumber());
        Assert.Equal("Void VerboseStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void VerboseStringException(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.VerboseStringException();
        var logEvent = verboses.Single();
        Assert.Equal(28, logEvent.LineNumber());
        Assert.Equal("Void VerboseStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsDebugEnabled(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsDebugEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Debug(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        var logEvent = debugs.Single();
        Assert.Equal(37, logEvent.LineNumber());
        Assert.Equal("Void Debug()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugString(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        var logEvent = debugs.Single();
        Assert.Equal(42, logEvent.LineNumber());
        Assert.Equal("Void DebugString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugStringParams(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        var logEvent = debugs.Single();
        Assert.Equal(47, logEvent.LineNumber());
        Assert.Equal("Void DebugStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void DebugStringException(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        var logEvent = debugs.Single();
        Assert.Equal(52, logEvent.LineNumber());
        Assert.Equal("Void DebugStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsInformationEnabled(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsInformationEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Information(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Information();
        var logEvent = informations.Single();
        Assert.Equal(62, logEvent.LineNumber());
        Assert.Equal("Void Information()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationString(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationString();
        var logEvent = informations.Single();
        Assert.Equal(67, logEvent.LineNumber());
        Assert.Equal("Void InformationString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationStringParams(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringParams();
        var logEvent = informations.Single();
        Assert.Equal(72, logEvent.LineNumber());
        Assert.Equal("Void InformationStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void InformationStringException(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringException();
        var logEvent = informations.Single();
        Assert.Equal(77, logEvent.LineNumber());
        Assert.Equal("Void InformationStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsWarningEnabled(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsWarningEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Warning(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Warning();
        var logEvent = warns.Single();
        Assert.Equal(87, logEvent.LineNumber());
        Assert.Equal("Void Warning()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningString(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        var logEvent = warns.Single();
        Assert.Equal(92, logEvent.LineNumber());
        Assert.Equal("Void WarningString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningStringParams(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        var logEvent = warns.Single();
        Assert.Equal(97, logEvent.LineNumber());
        Assert.Equal("Void WarningStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void WarningStringException(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        var logEvent = warns.Single();
        Assert.Equal(102, logEvent.LineNumber());
        Assert.Equal("Void WarningStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsErrorEnabled(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsErrorEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Error(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Error();
        var logEvent = errors.Single();
        Assert.Equal(112, logEvent.LineNumber());
        Assert.Equal("Void Error()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorString(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        var logEvent = errors.Single();
        Assert.Equal(117, logEvent.LineNumber());
        Assert.Equal("Void ErrorString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorStringParams(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        var logEvent = errors.Single();
        Assert.Equal(122, logEvent.LineNumber());
        Assert.Equal("Void ErrorStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void ErrorStringException(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        var logEvent = errors.Single();
        Assert.Equal(127, logEvent.LineNumber());
        Assert.Equal("Void ErrorStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void IsFatalEnabled(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        Assert.True(instance.IsFatalEnabled());
    }

    [Theory, MemberData(nameof(Targets))]
    public void Fatal(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Fatal();
        var logEvent = fatals.Single();
        Assert.Equal(137, logEvent.LineNumber());
        Assert.Equal("Void Fatal()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalString(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        var logEvent = fatals.Single();
        Assert.Equal(142, logEvent.LineNumber());
        Assert.Equal("Void FatalString()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalStringParams(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        var logEvent = fatals.Single();
        Assert.Equal(147, logEvent.LineNumber());
        Assert.Equal("Void FatalStringParams()", logEvent.MethodName());
        Assert.Equal("TheMessage {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void FatalStringException(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        var logEvent = fatals.Single();
        Assert.Equal(152, logEvent.LineNumber());
        Assert.Equal("Void FatalStringException()", logEvent.MethodName());
        Assert.Equal("TheMessage", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithLogging", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void PeVerify(string target)
    {
        Verifier.Verify(assemblies.GetBeforePath(target), assemblies.GetAfterPath(target));
    }

    [Theory, MemberData(nameof(Targets))]
    public void AsyncMethod(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncMethod();
        var logEvent = debugs.Single();
        Assert.Equal(11, logEvent.LineNumber());
        Assert.Equal("Void AsyncMethod()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void EnumeratorMethod(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        var logEvent = debugs.Single();
        Assert.Equal(17, logEvent.LineNumber());
        Assert.Equal("IEnumerable<Int32> EnumeratorMethod()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void DelegateMethod(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        var logEvent = debugs.Single();
        Assert.Equal(24, logEvent.LineNumber());
        Assert.Equal("Void DelegateMethod()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void AsyncDelegateMethod(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var logEvent = debugs.Single();
        Assert.Equal(39, logEvent.LineNumber());
        Assert.Equal("Void AsyncDelegateMethod()", logEvent.MethodName());
        Assert.Equal("", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Theory, MemberData(nameof(Targets))]
    public void LambdaMethod(string target)
    {
        var type = assemblies.GetAssembly(target).GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        var logEvent = debugs.Single();
        Assert.Equal(31, logEvent.LineNumber());
        Assert.Equal("Void LambdaMethod()", logEvent.MethodName());
        Assert.Equal("Foo {0}", logEvent.MessageTemplate.Text);
        Assert.Equal("ClassWithCompilerGeneratedClasses", logEvent.SourceContext());
    }

    [Fact(Skip = "need to fix ref")]
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
            yield return new object[] { "net462" };
            yield return new object[] { "netcoreapp1.1" };
        }
    }
}