using Anotar.Catel;
using Catel.Logging;

public class ClassWithExistingField
{
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