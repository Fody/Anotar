using System;
using NLog;
using NLog.Targets;

public sealed class ActionTarget: Target
{
    public Action<LogEventInfo> Action;
    protected override void Write(LogEventInfo logEvent)
    {
        Action(logEvent);
    }
}