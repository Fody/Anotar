using Anotar.Catel;
using Catel.Logging;

public class ClassWithExistingField
{
    static ILog existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogManager.GetCurrentClassLogger();
    }

    public void Debug()
    {
        LogTo.Debug();
    }

}