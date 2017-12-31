using Splat;

public class Template
{
    static IFullLogger existingLogger;

    static Template()
    {
        var service = (ILogManager)Locator.Current.GetService(typeof(ILogManager));
        existingLogger = service.GetLogger(typeof(Template));
    }

    public void Debug()
    {
        if (existingLogger.Level >= LogLevel.Debug)
        {
            existingLogger.Debug("df");
        }
    }
}