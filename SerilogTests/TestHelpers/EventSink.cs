using System;
using Serilog.Core;
using Serilog.Events;

public class EventSink : ILogEventSink
{

    public Action<LogEvent> Action;

    public void Emit(LogEvent logEvent)
    {
        Action(logEvent);
    }
}