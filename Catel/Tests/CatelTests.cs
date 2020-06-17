using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Catel.Logging;
using Fody;
using Xunit;

public class CatelTests
{
    static Assembly assembly;
    public static List<string> Errors = new List<string>();
    public static List<string> Debugs = new List<string>();
    public static List<string> Informations = new List<string>();
    public static List<string> Warnings = new List<string>();

    static CatelTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: new[] { "0x80131869" }).Assembly;

        LogManager.AddListener(new LogListener
        {
            Action = LogMessage
        });
    }

    public CatelTests()
    {
        Errors.Clear();
        Debugs.Clear();
        Informations.Clear();
        Warnings.Clear();
    }

    static void LogMessage(string message, LogEvent logEvent)
    {
        if (logEvent == LogEvent.Error)
        {
            Errors.Add(message);
            return;
        }

        if (logEvent == LogEvent.Warning)
        {
            Warnings.Add(message);
            return;
        }

        if (logEvent == LogEvent.Info)
        {
            Informations.Add(message);
            return;
        }

        if (logEvent == LogEvent.Debug)
        {
            Debugs.Add(message);
// ReSharper disable once RedundantJumpStatement
            return;
        }
    }

    [Fact]
    public void Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = Debugs.First();
        Assert.True(message.StartsWith("Method: 'Void Debug()'. Line: ~"), message);
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
    public void OnExceptionToWarning()
    {
        var expected = "Exception occurred in 'Void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        CheckException(action, Warnings, expected);
    }

    [Fact]
    public void OnExceptionToWarningWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        CheckException(action, Warnings, expected);
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
    public void MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        Assert.Equal("a", instance.MethodThatReturns("x", 6));
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
        Assert.Single(Warnings);
        Assert.StartsWith("Method: 'Void Warning()'. Line: ~", Warnings.First());
    }

    [Fact]
    public void WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        Assert.Single(Warnings);
        Assert.StartsWith("Method: 'Void WarningString()'. Line: ~", Warnings.First());
    }

    [Fact]
    public void WarningStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringFunc();
        Assert.Single(Warnings);
        Assert.StartsWith("Method: 'Void WarningStringFunc()'. Line: ~", Warnings.First());
    }

    [Fact]
    public void WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        Assert.Single(Warnings);
        Assert.StartsWith("Method: 'Void WarningStringParams()'. Line: ~", Warnings.First());
    }

    [Fact]
    public void WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        Assert.Single(Warnings);
        Assert.StartsWith("Method: 'Void WarningStringException()'. Line: ~", Warnings.First());
    }

    [Fact]
    public void WarningStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringExceptionFunc();
        Assert.Single(Warnings);
        Assert.StartsWith("Method: 'Void WarningStringExceptionFunc()'. Line: ~", Warnings.First());
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
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        Assert.Contains(Debugs, x => x.StartsWith("Method: 'Task AsyncMethod()'. Line: ~"));
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
        var message = Informations.First();
        Assert.True(message.StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~"), message);
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