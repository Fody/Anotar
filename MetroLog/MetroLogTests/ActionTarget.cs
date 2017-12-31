using System;
using MetroLog;
using MetroLog.Targets;

public class ActionTarget : SyncTarget
{
    public Action<LogEventInfo> Action;

    public ActionTarget() : base(null)
    {
    }

    protected override void Write(LogWriteContext context, LogEventInfo entry)
    {
        Action(entry);
    }
}