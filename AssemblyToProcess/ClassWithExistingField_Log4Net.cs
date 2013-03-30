using Anotar;
using log4net;

public class ClassWithExistingField
{
    static ILog existingLogger;

    static ClassWithExistingField()
    {
		existingLogger = LogManager.GetLogger("ClassWithExistingField");
    }

    public void Debug()
    {
        Log.Debug();
    }
}