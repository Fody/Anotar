using Anotar.Log4Net;
using log4net;

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
}
