using Anotar.Custom;

public class ClassWithExistingField
{
// ReSharper disable NotAccessedField.Local
    static Logger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LoggerFactory.GetLogger < ClassWithExistingField>();
    }

    public void Debug()
    {
        LogTo.Debug();
    }
}