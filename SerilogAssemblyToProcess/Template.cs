using Serilog;
using Serilog.Events;

public class Template
{
    static ILogger anotarLogger = Log.ForContext<ClassWithLogging>();
    ILogger forContext;

    public void Debug()
    {
        if (anotarLogger.IsEnabled(LogEventLevel.Debug))
        {
            anotarLogger
                .ForContext("MethodName", "Void Debug()", false)
                .ForContext("LineNumber", 13, false)
                .Debug("");
        }
    }


}