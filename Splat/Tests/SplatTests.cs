using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fody;
using Splat;
using Xunit;

public class SplatTests: IDisposable
{
    static Assembly assembly;
    static Logger currentLogger = new Logger();

    static SplatTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: new[] { "0x80131869" }).Assembly;

        Locator.CurrentMutable.Register(() => new FuncLogManager(GetLogger), typeof(ILogManager));
    }

    static IFullLogger GetLogger(Type arg)
    {
        return new WrappingFullLogger(currentLogger);
    }

    public void Dispose()
    {
        currentLogger.Clear();
    }

    [Fact]
    public void ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        Assert.Single(currentLogger.Errors);
        var error = currentLogger.Errors.First();
        Assert.Contains("Method: 'Void Method()'. Line: ~", error);
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
        var message = currentLogger.Debugs.First();
        Assert.Contains("Method: 'Void Debug()'. Line: ~", message);
    }

    [Fact]
    public void ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        Assert.Single(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static));
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        Assert.Single(currentLogger.Debugs);
        Assert.Contains("Method: 'Void Debug()'. Line: ~", currentLogger.Debugs.First());
    }

    // ReSharper disable once UnusedParameter.Local
    void CheckException(Action<object> action, List<(Exception exception,string message,LogLevel level)> list, string expected)
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
        var first = list.First().message;
        Assert.True(first.Contains(expected), first);
    }

    [Fact]
    public void OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
    }

    [Fact]
    public void OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        CheckException(action, currentLogger.Exceptions, expected);
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
        Assert.Single(currentLogger.Debugs);
        Assert.Contains("Method: 'Void Debug()'. Line: ~", currentLogger.Debugs.First());
    }

    [Fact]
    public void DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        Assert.Single(currentLogger.Debugs);
        Assert.Contains("Method: 'Void DebugString()'. Line: ~", currentLogger.Debugs.First());
    }

    [Fact]
    public void DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        Assert.Single(currentLogger.Debugs);
        Assert.Contains("Method: 'Void DebugStringFunc()'. Line: ~", currentLogger.Debugs.First());
    }

    [Fact]
    public void DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        Assert.Single(currentLogger.Debugs);
        Assert.Contains("Method: 'Void DebugStringParams()'. Line: ~", currentLogger.Debugs.First());
    }

    [Fact]
    public void DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void DebugStringException()'. Line: ~", error.message);
    }

    [Fact]
    public void DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void DebugStringExceptionFunc()'. Line: ~", error.message);
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
        Assert.Single(currentLogger.Informations);
        Assert.Contains("Method: 'Void Info()'. Line: ~", currentLogger.Informations.First());
    }

    [Fact]
    public void InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        Assert.Single(currentLogger.Informations);
        Assert.Contains("Method: 'Void InfoString()'. Line: ~", currentLogger.Informations.First());
    }

    [Fact]
    public void InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        Assert.Single(currentLogger.Informations);
        Assert.Contains("Method: 'Void InfoStringFunc()'. Line: ~", currentLogger.Informations.First());
    }

    [Fact]
    public void InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        Assert.Single(currentLogger.Informations);
        Assert.Contains("Method: 'Void InfoStringParams()'. Line: ~", currentLogger.Informations.First());
    }

    [Fact]
    public void InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void InfoStringException()'. Line: ~", error.message);
    }

    [Fact]
    public void InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void InfoStringExceptionFunc()'. Line: ~", error.message);
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
        Assert.Single(currentLogger.Warns);
        Assert.Contains("Method: 'Void Warn()'. Line: ~", currentLogger.Warns.First());
    }

    [Fact]
    public void WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        Assert.Single(currentLogger.Warns);
        Assert.Contains("Method: 'Void WarnString()'. Line: ~", currentLogger.Warns.First());
    }

    [Fact]
    public void WarnStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringFunc();
        Assert.Single(currentLogger.Warns);
        Assert.Contains("Method: 'Void WarnStringFunc()'. Line: ~", currentLogger.Warns.First());
    }

    [Fact]
    public void WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        Assert.Single(currentLogger.Warns);
        Assert.Contains("Method: 'Void WarnStringParams()'. Line: ~", currentLogger.Warns.First());
    }

    [Fact]
    public void WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void WarnStringException()'. Line: ~", error.message);
    }

    [Fact]
    public void WarnStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void WarnStringExceptionFunc()'. Line: ~", error.message);
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
        Assert.Single(currentLogger.Errors);
        Assert.Contains("Method: 'Void Error()'. Line: ~", currentLogger.Errors.First());
    }

    [Fact]
    public void ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        Assert.Single(currentLogger.Errors);
        Assert.Contains("Method: 'Void ErrorString()'. Line: ~", currentLogger.Errors.First());
    }

    [Fact]
    public void ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        Assert.Single(currentLogger.Errors);
        Assert.Contains("Method: 'Void ErrorStringFunc()'. Line: ~", currentLogger.Errors.First());
    }

    [Fact]
    public void ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        Assert.Single(currentLogger.Errors);
        Assert.Contains("Method: 'Void ErrorStringParams()'. Line: ~", currentLogger.Errors.First());
    }

    [Fact]
    public void ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void ErrorStringException()'. Line: ~", error.message);
    }

    [Fact]
    public void ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void ErrorStringExceptionFunc()'. Line: ~", error.message);
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
        Assert.Single(currentLogger.Fatals);
        Assert.Contains("Method: 'Void Fatal()'. Line: ~", currentLogger.Fatals.First());
    }

    [Fact]
    public void FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        Assert.Single(currentLogger.Fatals);
        Assert.Contains("Method: 'Void FatalString()'. Line: ~", currentLogger.Fatals.First());
    }

    [Fact]
    public void FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        Assert.Single(currentLogger.Fatals);
        Assert.Contains("Method: 'Void FatalStringFunc()'. Line: ~", currentLogger.Fatals.First());
    }

    [Fact]
    public void FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        Assert.Single(currentLogger.Fatals);
        Assert.Contains("Method: 'Void FatalStringParams()'. Line: ~", currentLogger.Fatals.First());
    }

    [Fact]
    public void FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void FatalStringException()'. Line: ~", error.message);
    }

    [Fact]
    public void FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        Assert.Contains("Method: 'Void FatalStringExceptionFunc()'. Line: ~", error.message);
    }

    [Fact]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        Assert.Single(currentLogger.Debugs);
        Assert.Contains("Method: 'Task AsyncMethod()'. Line: ~", currentLogger.Debugs.First());
    }

    [Fact]
    public void EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        Assert.Single(currentLogger.Debugs);
        Assert.True(currentLogger.Debugs.First().Contains("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~"), currentLogger.Debugs.First());
    }

    [Fact]
    public void DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        Assert.Single(currentLogger.Debugs);
        Assert.True(currentLogger.Debugs.First().Contains("Method: 'Void DelegateMethod()'. Line: ~"), currentLogger.Debugs.First());
    }

    [Fact]
    public void AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        Assert.Single(currentLogger.Debugs);
        Assert.True(currentLogger.Debugs.First().Contains("Method: 'Void AsyncDelegateMethod()'. Line: ~"), currentLogger.Debugs.First());
    }

    [Fact]
    public void LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        Assert.Single(currentLogger.Debugs);
        Assert.True(currentLogger.Debugs.First().Contains("Method: 'Void LambdaMethod()'. Line: ~"), currentLogger.Debugs.First());
    }
}