using Anotar.Custom;

public class ClassWithExistingField
{
    static Logger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LoggerFactory.GetLogger < ClassWithExistingField>();
    }

    public void Debug()
    {
        Log.Debug();
    }
}