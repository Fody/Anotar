using System;
using Catel.Logging;
using Scalpel;

[Remove]
public class LogListener : LogListenerBase
{
    public Action<string,LogEvent> Action;

    protected override void Write(ILog log, string message, LogEvent logEvent, object extraData)
    {
        Action(message, logEvent);
    }
}