using NLog;
using Target = NLog.Targets.Target;

public sealed class ActionTarget: Target
{
    public Action<LogEventInfo> Action;
    protected override void Write(LogEventInfo logEvent)
    {
        Action(logEvent);
    }
}