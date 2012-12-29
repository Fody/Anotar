using Anotar;
using log4net;

public class ClassWithExistingField
{
    static ILog existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogManager.GetLogger("OnException");
    }

    public void Debug()
    {
        Log.Debug();
    }
}