using Anotar.NLog;
using NLog;

public class ClassWithExistingField
{
// ReSharper disable once NotAccessedField.Local
    static Logger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogManager.GetLogger("ClassWithExistingField");
    }

    public void Debug()
    {
        LogTo.Debug();
    }
}