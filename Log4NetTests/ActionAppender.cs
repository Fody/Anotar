using System;
using log4net.Appender;
using log4net.Core;

public sealed class ActionAppender : AppenderSkeleton
{
    public Action<LoggingEvent> Action;

    protected override void Append(LoggingEvent loggingEvent)
    {
        Action(loggingEvent);
    }
}