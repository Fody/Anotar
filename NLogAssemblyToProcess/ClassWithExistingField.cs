using Anotar.NLog;
using NLog;

public class ClassWithExistingField
{
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