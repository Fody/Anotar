using Anotar.LibLog;
using LibLogAssembly.Logging;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static ILog existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogProvider.For<ClassWithExistingField>();
    }

    public void Debug()
    {
        LogTo.Debug("Sdf","asd");
    }
    public void Debug2()
    {
        existingLogger.DebugFormat("Sdf","asd");
    }

}