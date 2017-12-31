using Anotar.MetroLog;
using MetroLog;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static ILogger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogManagerFactory.DefaultLogManager.GetLogger("ClassWithExistingField");
    }

    public void Debug()
    {
        LogTo.Debug();
    }
}