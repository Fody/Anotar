using System;
using Scalpel;
using log4net.Appender;
using log4net.Core;

[Remove]
public class ActionAppender : AppenderSkeleton
{
    public Action<LoggingEvent> Action;

    protected override void Append(LoggingEvent loggingEvent)
    {
        Action(loggingEvent);
    }
}