using System.Reflection;
using Fody;
using Splat;

// tests share static logger state
[NotInParallel]
public class SplatTests: IDisposable
{
    static Assembly assembly;
    static Logger currentLogger = new();

    static SplatTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: ["0x80131869"]).Assembly;

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

    [Test]
    public async Task ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        await Assert.That(currentLogger.Errors).HasSingleItem();
        var error = currentLogger.Errors.First();
        await Assert.That(error).Contains("Method: 'Void Method()'. Line: ~");
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
        var message = currentLogger.Debugs.First();
        await Assert.That(message).Contains("Method: 'Void Debug()'. Line: ~");
    }

    [Test]
    public async Task ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        await Assert.That(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static)).HasSingleItem();
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void Debug()'. Line: ~");
    }

    // ReSharper disable once UnusedParameter.Local
    static async Task CheckException(Action<object> action, List<(Exception exception,string message,LogLevel level)> list, string expected)
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
        var first = list.First().message;
        await Assert.That(first).Contains(expected);
    }

    [Test]
    public async Task OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToInfo()
    {
        var expected = "Exception occurred in 'Void ToInfo(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfo("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToInfoWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInfoWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInfoWithReturn("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToWarn()
    {
        var expected = "Exception occurred in 'Void ToWarn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarn("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToWarnWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarnWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarnWithReturn("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
    }

    [Test]
    public async Task OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        await CheckException(action, currentLogger.Exceptions, expected);
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
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void Debug()'. Line: ~");
    }

    [Test]
    public async Task DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void DebugString()'. Line: ~");
    }

    [Test]
    public async Task DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void DebugStringFunc()'. Line: ~");
    }

    [Test]
    public async Task DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void DebugStringParams()'. Line: ~");
    }

    [Test]
    public async Task DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void DebugStringException()'. Line: ~");
    }

    [Test]
    public async Task DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void DebugStringExceptionFunc()'. Line: ~");
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
        await Assert.That(currentLogger.Informations).HasSingleItem();
        await Assert.That(currentLogger.Informations.First()).Contains("Method: 'Void Info()'. Line: ~");
    }

    [Test]
    public async Task InfoString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoString();
        await Assert.That(currentLogger.Informations).HasSingleItem();
        await Assert.That(currentLogger.Informations.First()).Contains("Method: 'Void InfoString()'. Line: ~");
    }

    [Test]
    public async Task InfoStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringFunc();
        await Assert.That(currentLogger.Informations).HasSingleItem();
        await Assert.That(currentLogger.Informations.First()).Contains("Method: 'Void InfoStringFunc()'. Line: ~");
    }

    [Test]
    public async Task InfoStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringParams();
        await Assert.That(currentLogger.Informations).HasSingleItem();
        await Assert.That(currentLogger.Informations.First()).Contains("Method: 'Void InfoStringParams()'. Line: ~");
    }

    [Test]
    public async Task InfoStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringException();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void InfoStringException()'. Line: ~");
    }

    [Test]
    public async Task InfoStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InfoStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void InfoStringExceptionFunc()'. Line: ~");
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
        await Assert.That(currentLogger.Warns).HasSingleItem();
        await Assert.That(currentLogger.Warns.First()).Contains("Method: 'Void Warn()'. Line: ~");
    }

    [Test]
    public async Task WarnString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnString();
        await Assert.That(currentLogger.Warns).HasSingleItem();
        await Assert.That(currentLogger.Warns.First()).Contains("Method: 'Void WarnString()'. Line: ~");
    }

    [Test]
    public async Task WarnStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringFunc();
        await Assert.That(currentLogger.Warns).HasSingleItem();
        await Assert.That(currentLogger.Warns.First()).Contains("Method: 'Void WarnStringFunc()'. Line: ~");
    }

    [Test]
    public async Task WarnStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringParams();
        await Assert.That(currentLogger.Warns).HasSingleItem();
        await Assert.That(currentLogger.Warns.First()).Contains("Method: 'Void WarnStringParams()'. Line: ~");
    }

    [Test]
    public async Task WarnStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringException();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void WarnStringException()'. Line: ~");
    }

    [Test]
    public async Task WarnStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarnStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void WarnStringExceptionFunc()'. Line: ~");
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
        await Assert.That(currentLogger.Errors).HasSingleItem();
        await Assert.That(currentLogger.Errors.First()).Contains("Method: 'Void Error()'. Line: ~");
    }

    [Test]
    public async Task ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        await Assert.That(currentLogger.Errors).HasSingleItem();
        await Assert.That(currentLogger.Errors.First()).Contains("Method: 'Void ErrorString()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        await Assert.That(currentLogger.Errors).HasSingleItem();
        await Assert.That(currentLogger.Errors.First()).Contains("Method: 'Void ErrorStringFunc()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        await Assert.That(currentLogger.Errors).HasSingleItem();
        await Assert.That(currentLogger.Errors.First()).Contains("Method: 'Void ErrorStringParams()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void ErrorStringException()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void ErrorStringExceptionFunc()'. Line: ~");
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
        await Assert.That(currentLogger.Fatals).HasSingleItem();
        await Assert.That(currentLogger.Fatals.First()).Contains("Method: 'Void Fatal()'. Line: ~");
    }

    [Test]
    public async Task FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        await Assert.That(currentLogger.Fatals).HasSingleItem();
        await Assert.That(currentLogger.Fatals.First()).Contains("Method: 'Void FatalString()'. Line: ~");
    }

    [Test]
    public async Task FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        await Assert.That(currentLogger.Fatals).HasSingleItem();
        await Assert.That(currentLogger.Fatals.First()).Contains("Method: 'Void FatalStringFunc()'. Line: ~");
    }

    [Test]
    public async Task FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        await Assert.That(currentLogger.Fatals).HasSingleItem();
        await Assert.That(currentLogger.Fatals.First()).Contains("Method: 'Void FatalStringParams()'. Line: ~");
    }

    [Test]
    public async Task FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void FatalStringException()'. Line: ~");
    }

    [Test]
    public async Task FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        var error = currentLogger.Exceptions.Single();
        await Assert.That(error.message).Contains("Method: 'Void FatalStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Task AsyncMethod()'. Line: ~");
    }

    [Test]
    public async Task EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~");
    }

    [Test]
    public async Task DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void DelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void AsyncDelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        await Assert.That(currentLogger.Debugs).HasSingleItem();
        await Assert.That(currentLogger.Debugs.First()).Contains("Method: 'Void LambdaMethod()'. Line: ~");
    }
}