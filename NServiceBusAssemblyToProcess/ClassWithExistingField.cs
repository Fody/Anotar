using Anotar.NServiceBus;
using NServiceBus.Logging;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static ILog existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogManager.GetLogger("ClassWithExistingField");
    }

    public void Debug()
    {
        LogTo.Debug();
    }
}


public class GenericClass2<T>
{
    private static ILog AnotarLogger = LogManager.GetLogger("GenericClass2`1");
    public void Debug()
    {
        AnotarLogger.DebugFormat("Method: 'Void Debug()'. Line: ~7. ", new object[0]);
    }
}