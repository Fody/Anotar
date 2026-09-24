using System.Reflection;
using Fody;

// tests share static logger state
[NotInParallel]
public class CustomTests
{
    static Assembly assembly;

    static CustomTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: ["0x80131869"]).Assembly;
    }

    public CustomTests()
    {
        LoggerFactory.Clear();
    }

    [Test]
    public async Task Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        var message = LoggerFactory.DebugEntries.First();
        await Assert.That(message.Format).StartsWith("Method: 'Void Debug()'. Line: ~");
    }


    [Test]
    public async Task ClassWithComplexExpressionInLog()
    {
        var type = assembly.GetType("ClassWithComplexExpressionInLog");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Method();
        await Assert.That(LoggerFactory.ErrorEntries).HasSingleItem();
        await Assert.That(LoggerFactory.ErrorEntries.First().Format).StartsWith("Method: 'Void Method()'. Line: ~");
    }

    [Test]
    public async Task EnsureLoggerFactoryAttributeIsRemoved()
    {
        var first = assembly.GetCustomAttributes(false).FirstOrDefault(_ => _.GetType().Name.Contains("LoggerFactoryAttribute"));
        await Assert.That(first).IsNull();
    }

    [Test]
    public async Task MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        await Assert.That((string) instance.MethodThatReturns("x", 6)).IsEqualTo("a");
    }

    [Test]
    public async Task ClassWithExistingField()
    {
        var type = assembly.GetType("ClassWithExistingField");
        await Assert.That(type.GetFields(BindingFlags.NonPublic | BindingFlags.Static)).HasSingleItem();
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.Debug();
        await Assert.That(LoggerFactory.DebugEntries).HasSingleItem();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void Debug()'. Line: ~");
    }

    // ReSharper disable once UnusedParameter.Local
    static async Task CheckException(Action<object> action, List<LogEntry> list, string expected)
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
        var message = first.Format;
        await Assert.That(message).StartsWith(expected);
    }

    [Test]
    public async Task OnExceptionToTrace()
    {
        var expected = "Exception occurred in 'Void ToTrace(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTrace("x", 6);
        await CheckException(action, LoggerFactory.TraceEntries, expected);
    }

    [Test]
    public async Task OnExceptionToTraceWithReturn()
    {
        var expected = "Exception occurred in 'Object ToTraceWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToTraceWithReturn("x", 6);
        await CheckException(action, LoggerFactory.TraceEntries, expected);
    }

    [Test]
    public async Task OnExceptionToDebug()
    {
        var expected = "Exception occurred in 'Void ToDebug(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebug("x", 6);
        await CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Test]
    public async Task OnExceptionToDebugWithReturn()
    {
        var expected = "Exception occurred in 'Object ToDebugWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToDebugWithReturn("x", 6);
        await CheckException(action, LoggerFactory.DebugEntries, expected);
    }

    [Test]
    public async Task OnExceptionToInformation()
    {
        var expected = "Exception occurred in 'Void ToInformation(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformation("x", 6);
        await CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Test]
    public async Task OnExceptionToInformationWithReturn()
    {
        var expected = "Exception occurred in 'Object ToInformationWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToInformationWithReturn("x", 6);
        await CheckException(action, LoggerFactory.InformationEntries, expected);
    }

    [Test]
    public async Task OnExceptionToWarning()
    {
        var expected = "Exception occurred in 'Void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        await CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Test]
    public async Task OnExceptionToWarningWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        await CheckException(action, LoggerFactory.WarningEntries, expected);
    }

    [Test]
    public async Task OnExceptionToError()
    {
        var expected = "Exception occurred in 'Void ToError(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToError("x", 6);
        await CheckException(action, LoggerFactory.ErrorEntries, expected);
    }

    [Test]
    public async Task OnExceptionToErrorWithReturn()
    {
        var expected = "Exception occurred in 'Object ToErrorWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToErrorWithReturn("x", 6);
        await CheckException(action, LoggerFactory.ErrorEntries, expected);
    }

    [Test]
    public async Task OnExceptionToFatal()
    {
        var expected = "Exception occurred in 'Void ToFatal(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatal("x", 6);
        await CheckException(action, LoggerFactory.FatalEntries, expected);
    }

    [Test]
    public async Task OnExceptionToFatalWithReturn()
    {
        var expected = "Exception occurred in 'Object ToFatalWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToFatalWithReturn("x", 6);
        await CheckException(action, LoggerFactory.FatalEntries, expected);
    }

    [Test]
    public async Task IsTraceEnabled()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        await Assert.That((bool) instance.IsTraceEnabled()).IsTrue();
    }

    [Test]
    public async Task TraceString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceString();
        await Assert.That(LoggerFactory.TraceEntries).HasSingleItem();
        await Assert.That(LoggerFactory.TraceEntries.First().Format).StartsWith("Method: 'Void TraceString()'. Line: ~");
    }

    [Test]
    public async Task TraceStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringFunc();
        await Assert.That(LoggerFactory.TraceEntries).HasSingleItem();
        await Assert.That(LoggerFactory.TraceEntries.First().Format).StartsWith("Method: 'Void TraceStringFunc()'. Line: ~");
    }

    [Test]
    public async Task TraceStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringParams();
        await Assert.That(LoggerFactory.TraceEntries).HasSingleItem();
        await Assert.That(LoggerFactory.TraceEntries.First().Format).StartsWith("Method: 'Void TraceStringParams()'. Line: ~");
    }

    [Test]
    public async Task TraceStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringException();
        await Assert.That(LoggerFactory.TraceEntries).HasSingleItem();
        await Assert.That(LoggerFactory.TraceEntries.First().Format).StartsWith("Method: 'Void TraceStringException()'. Line: ~");
    }

    [Test]
    public async Task TraceStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.TraceStringExceptionFunc();
        await Assert.That(LoggerFactory.TraceEntries).HasSingleItem();
        await Assert.That(LoggerFactory.TraceEntries.First().Format).StartsWith("Method: 'Void TraceStringExceptionFunc()'. Line: ~");
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
        await Assert.That(LoggerFactory.DebugEntries).HasSingleItem();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void Debug()'. Line: ~");
    }

    [Test]
    public async Task DebugString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugString();
        await Assert.That(LoggerFactory.DebugEntries).HasSingleItem();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void DebugString()'. Line: ~");
    }

    [Test]
    public async Task DebugStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringFunc();
        await Assert.That(LoggerFactory.DebugEntries).HasSingleItem();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void DebugStringFunc()'. Line: ~");
    }

    [Test]
    public async Task DebugStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringParams();
        await Assert.That(LoggerFactory.DebugEntries).HasSingleItem();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void DebugStringParams()'. Line: ~");
    }

    [Test]
    public async Task DebugStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringException();
        await Assert.That(LoggerFactory.DebugEntries).HasSingleItem();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void DebugStringException()'. Line: ~");
    }

    [Test]
    public async Task DebugStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DebugStringExceptionFunc();
        await Assert.That(LoggerFactory.DebugEntries).HasSingleItem();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void DebugStringExceptionFunc()'. Line: ~");
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
        await Assert.That(LoggerFactory.InformationEntries).HasSingleItem();
        await Assert.That(LoggerFactory.InformationEntries.First().Format).StartsWith("Method: 'Void Information()'. Line: ~");
    }

    [Test]
    public async Task InformationString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationString();
        await Assert.That(LoggerFactory.InformationEntries).HasSingleItem();
        await Assert.That(LoggerFactory.InformationEntries.First().Format).StartsWith("Method: 'Void InformationString()'. Line: ~");
    }

    [Test]
    public async Task InformationStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringFunc();
        await Assert.That(LoggerFactory.InformationEntries).HasSingleItem();
        await Assert.That(LoggerFactory.InformationEntries.First().Format).StartsWith("Method: 'Void InformationStringFunc()'. Line: ~");
    }

    [Test]
    public async Task InformationStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringParams();
        await Assert.That(LoggerFactory.InformationEntries).HasSingleItem();
        await Assert.That(LoggerFactory.InformationEntries.First().Format).StartsWith("Method: 'Void InformationStringParams()'. Line: ~");
    }

    [Test]
    public async Task InformationStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringException();
        await Assert.That(LoggerFactory.InformationEntries).HasSingleItem();
        await Assert.That(LoggerFactory.InformationEntries.First().Format).StartsWith("Method: 'Void InformationStringException()'. Line: ~");
    }

    [Test]
    public async Task InformationStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.InformationStringExceptionFunc();
        await Assert.That(LoggerFactory.InformationEntries).HasSingleItem();
        await Assert.That(LoggerFactory.InformationEntries.First().Format).StartsWith("Method: 'Void InformationStringExceptionFunc()'. Line: ~");
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
        await Assert.That(LoggerFactory.WarningEntries).HasSingleItem();
        await Assert.That(LoggerFactory.WarningEntries.First().Format).StartsWith("Method: 'Void Warning()'. Line: ~");
    }

    [Test]
    public async Task WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        await Assert.That(LoggerFactory.WarningEntries).HasSingleItem();
        await Assert.That(LoggerFactory.WarningEntries.First().Format).StartsWith("Method: 'Void WarningString()'. Line: ~");
    }

    [Test]
    public async Task WarningStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringFunc();
        await Assert.That(LoggerFactory.WarningEntries).HasSingleItem();
        await Assert.That(LoggerFactory.WarningEntries.First().Format).StartsWith("Method: 'Void WarningStringFunc()'. Line: ~");
    }

    [Test]
    public async Task WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        await Assert.That(LoggerFactory.WarningEntries).HasSingleItem();
        await Assert.That(LoggerFactory.WarningEntries.First().Format).StartsWith("Method: 'Void WarningStringParams()'. Line: ~");
    }

    [Test]
    public async Task WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        await Assert.That(LoggerFactory.WarningEntries).HasSingleItem();
        await Assert.That(LoggerFactory.WarningEntries.First().Format).StartsWith("Method: 'Void WarningStringException()'. Line: ~");
    }

    [Test]
    public async Task WarningStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringExceptionFunc();
        await Assert.That(LoggerFactory.WarningEntries).HasSingleItem();
        await Assert.That(LoggerFactory.WarningEntries.First().Format).StartsWith("Method: 'Void WarningStringExceptionFunc()'. Line: ~");
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
        await Assert.That(LoggerFactory.ErrorEntries).HasSingleItem();
        await Assert.That(LoggerFactory.ErrorEntries.First().Format).StartsWith("Method: 'Void Error()'. Line: ~");
    }

    [Test]
    public async Task ErrorString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorString();
        await Assert.That(LoggerFactory.ErrorEntries).HasSingleItem();
        await Assert.That(LoggerFactory.ErrorEntries.First().Format).StartsWith("Method: 'Void ErrorString()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringFunc();
        await Assert.That(LoggerFactory.ErrorEntries).HasSingleItem();
        await Assert.That(LoggerFactory.ErrorEntries.First().Format).StartsWith("Method: 'Void ErrorStringFunc()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringParams();
        await Assert.That(LoggerFactory.ErrorEntries).HasSingleItem();
        await Assert.That(LoggerFactory.ErrorEntries.First().Format).StartsWith("Method: 'Void ErrorStringParams()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringException();
        await Assert.That(LoggerFactory.ErrorEntries).HasSingleItem();
        await Assert.That(LoggerFactory.ErrorEntries.First().Format).StartsWith("Method: 'Void ErrorStringException()'. Line: ~");
    }

    [Test]
    public async Task ErrorStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.ErrorStringExceptionFunc();
        await Assert.That(LoggerFactory.ErrorEntries).HasSingleItem();
        await Assert.That(LoggerFactory.ErrorEntries.First().Format).StartsWith("Method: 'Void ErrorStringExceptionFunc()'. Line: ~");
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
        await Assert.That(LoggerFactory.FatalEntries).HasSingleItem();
        await Assert.That(LoggerFactory.FatalEntries.First().Format).StartsWith("Method: 'Void Fatal()'. Line: ~");
    }

    [Test]
    public async Task FatalString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalString();
        await Assert.That(LoggerFactory.FatalEntries).HasSingleItem();
        await Assert.That(LoggerFactory.FatalEntries.First().Format).StartsWith("Method: 'Void FatalString()'. Line: ~");
    }

    [Test]
    public async Task FatalStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringFunc();
        await Assert.That(LoggerFactory.FatalEntries).HasSingleItem();
        await Assert.That(LoggerFactory.FatalEntries.First().Format).StartsWith("Method: 'Void FatalStringFunc()'. Line: ~");
    }

    [Test]
    public async Task FatalStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringParams();
        await Assert.That(LoggerFactory.FatalEntries).HasSingleItem();
        await Assert.That(LoggerFactory.FatalEntries.First().Format).StartsWith("Method: 'Void FatalStringParams()'. Line: ~");
    }

    [Test]
    public async Task FatalStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringException();
        await Assert.That(LoggerFactory.FatalEntries).HasSingleItem();
        await Assert.That(LoggerFactory.FatalEntries.First().Format).StartsWith("Method: 'Void FatalStringException()'. Line: ~");
    }

    [Test]
    public async Task FatalStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.FatalStringExceptionFunc();
        await Assert.That(LoggerFactory.FatalEntries).HasSingleItem();
        await Assert.That(LoggerFactory.FatalEntries.First().Format).StartsWith("Method: 'Void FatalStringExceptionFunc()'. Line: ~");
    }

    [Test]
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncMethod();
        await Assert.That(LoggerFactory.DebugEntries.First().Format).StartsWith("Method: 'Void AsyncMethod()'. Line: ~");
    }

    [Test]
    public async Task EnumeratorMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        ((IEnumerable<int>) instance.EnumeratorMethod()).ToList();
        var message = LoggerFactory.DebugEntries.First().Format;
        await Assert.That(message).StartsWith("Method: 'IEnumerable<Int32> EnumeratorMethod()'. Line: ~");
    }

    [Test]
    public async Task DelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.DelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        await Assert.That(message).StartsWith("Method: 'Void DelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task AsyncDelegateMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.AsyncDelegateMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        await Assert.That(message).StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~");
    }

    [Test]
    public async Task LambdaMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.LambdaMethod();
        var message = LoggerFactory.DebugEntries.First().Format;
        await Assert.That(message).StartsWith("Method: 'Void LambdaMethod()'. Line: ~");
    }
}