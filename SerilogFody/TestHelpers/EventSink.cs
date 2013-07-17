using System;
using Scalpel;
using Serilog.Core;
using Serilog.Events;

[Remove]
public class EventSink : ILogEventSink
{
	
    public Action<LogEvent> Action;
	public void Emit(LogEvent logEvent)
	{
		Action(logEvent);
	}
}