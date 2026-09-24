using System.Reflection;
using Catel.Logging;
using Fody;

// tests share static logger state
[NotInParallel]
public class CatelTests
{
    static Assembly assembly;
    public static List<string> Errors = new();
    public static List<string> Debugs = new();
    public static List<string> Informations = new();
    public static List<string> Warnings = new();

    static CatelTests()
    {
        var moduleWeaver = new ModuleWeaver();
        assembly = moduleWeaver.ExecuteTestRun(
            assemblyPath: "AssemblyToProcess.dll",
            ignoreCodes: ["0x80131869"]).Assembly;

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

    [Test]
    public async Task Generic()
    {
        var type = assembly.GetType("GenericClass`1");
        var constructedType = type.MakeGenericType(typeof(string));
        var instance = (dynamic) Activator.CreateInstance(constructedType);
        instance.Debug();
        // Catel may log its own internal messages, so look for the woven one
        await Assert.That(Debugs.Any(_ => _.StartsWith("Method: 'Void Debug()'. Line: ~"))).IsTrue();
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
    public async Task OnExceptionToWarning()
    {
        var expected = "Exception occurred in 'Void ToWarning(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarning("x", 6);
        await CheckException(action, Warnings, expected);
    }

    [Test]
    public async Task OnExceptionToWarningWithReturn()
    {
        var expected = "Exception occurred in 'Object ToWarningWithReturn(String, Int32)'.  param1 'x' param2 '6'";
        Action<dynamic> action = o => o.ToWarningWithReturn("x", 6);
        await CheckException(action, Warnings, expected);
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
    public async Task MethodThatReturns()
    {
        var type = assembly.GetType("OnException");
        var instance = (dynamic) Activator.CreateInstance(type);

        await Assert.That((string) instance.MethodThatReturns("x", 6)).IsEqualTo("a");
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
        await Assert.That(Warnings).HasSingleItem();
        await Assert.That(Warnings.First()).StartsWith("Method: 'Void Warning()'. Line: ~");
    }

    [Test]
    public async Task WarningString()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningString();
        await Assert.That(Warnings).HasSingleItem();
        await Assert.That(Warnings.First()).StartsWith("Method: 'Void WarningString()'. Line: ~");
    }

    [Test]
    public async Task WarningStringFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringFunc();
        await Assert.That(Warnings).HasSingleItem();
        await Assert.That(Warnings.First()).StartsWith("Method: 'Void WarningStringFunc()'. Line: ~");
    }

    [Test]
    public async Task WarningStringParams()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringParams();
        await Assert.That(Warnings).HasSingleItem();
        await Assert.That(Warnings.First()).StartsWith("Method: 'Void WarningStringParams()'. Line: ~");
    }

    [Test]
    public async Task WarningStringException()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringException();
        await Assert.That(Warnings).HasSingleItem();
        await Assert.That(Warnings.First()).StartsWith("Method: 'Void WarningStringException()'. Line: ~");
    }

    [Test]
    public async Task WarningStringExceptionFunc()
    {
        var type = assembly.GetType("ClassWithLogging");
        var instance = (dynamic) Activator.CreateInstance(type);
        instance.WarningStringExceptionFunc();
        await Assert.That(Warnings).HasSingleItem();
        await Assert.That(Warnings.First()).StartsWith("Method: 'Void WarningStringExceptionFunc()'. Line: ~");
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
    public async Task AsyncMethod()
    {
        var type = assembly.GetType("ClassWithCompilerGeneratedClasses");
        var instance = (dynamic) Activator.CreateInstance(type);
        Task task = instance.AsyncMethod();
        await task;
        await Assert.That(Debugs.Any(_ => _.StartsWith("Method: 'Task AsyncMethod()'. Line: ~"))).IsTrue();
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
        var message = Informations.First();
        await Assert.That(message).StartsWith("Method: 'Void AsyncDelegateMethod()'. Line: ~");
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