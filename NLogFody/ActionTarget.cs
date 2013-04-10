using System;
using NLog;
using NLog.Targets;
using Scalpel;

[Remove]
public sealed class ActionTarget: Target
{
    public Action<LogEventInfo> Action;
    protected override void Write(LogEventInfo logEvent)
    {
        Action(logEvent);
    } 
}