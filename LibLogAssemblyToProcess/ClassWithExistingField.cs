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
        LogTo.Debug("Sdf{0}","asd");
    }

}