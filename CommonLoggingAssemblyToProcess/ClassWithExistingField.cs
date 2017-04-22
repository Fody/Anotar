using Anotar.CommonLogging;
using Common.Logging;

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