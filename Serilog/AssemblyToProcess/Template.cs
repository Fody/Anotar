using System;
using System.Threading;
using Serilog;
using Serilog.Events;
#pragma warning disable 169

public class Template
{
    static Lazy<ILogger> lazyAnotarLogger = new Lazy<ILogger>(
        Log.ForContext<ClassWithLogging>,
        LazyThreadSafetyMode.ExecutionAndPublication);

    public void Debug()
    {
        if (lazyAnotarLogger.Value.IsEnabled(LogEventLevel.Debug))
        {
            lazyAnotarLogger.Value
                .Debug("");
        }
    }
}