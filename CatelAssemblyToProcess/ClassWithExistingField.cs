using System;
using Anotar.Catel;
using Catel.Logging;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static ILog existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogManager.GetLogger(typeof(ClassWithExistingField));
    }

    public void Debug()
    {
        LogTo.Debug();
    }

    public void Debug2()
    {
        var arg_12_0 = new Exception();
        
        var args = new object[0];
        var exception = arg_12_0;

        var func = new Func<string>(() => "sdfsjkdnf");
        var message = func();
        existingLogger.WriteWithData(exception, string.Concat("Method: 'Void DebugStringException()'. Line: ~38. ", message), null, LogEvent.Debug);
    }

}