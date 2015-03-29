using Serilog;
using Serilog.Events;

public class Template
{
    private static ILogger AnotarLogger = Log.ForContext<ClassWithLogging>();
    ILogger forContext;

    public void Debug()
    {
        if (AnotarLogger.IsEnabled(LogEventLevel.Debug))
        {
            AnotarLogger
                .ForContext("MethodName", "Void Debug()", false)
                .ForContext("LineNumber", 13, false)
                .Debug("", new object[0]);
        }
    }
}