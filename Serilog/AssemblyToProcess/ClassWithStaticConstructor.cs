using System;
using Anotar.Serilog;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Log = Serilog.Log;

public class ClassWithStaticConstructor
{
    public static string Message;

    public class EventSink :
        ILogEventSink
    {
        public Action<LogEvent> Action;

        public void Emit(LogEvent logEvent)
        {
            Action(logEvent);
        }
    }

    static ClassWithStaticConstructor()
    {
        var eventSink = new EventSink
        {
            Action = LogEvent
        };
        var log = new LoggerConfiguration()
            .WriteTo.Sink(eventSink)
            .CreateLogger();
        Log.Logger = log;

        LogTo.Information("log from static constructor");
    }

    static void LogEvent(LogEvent eventInfo)
    {
        Message = eventInfo.MessageTemplate.Text;
    }

    public static void StaticMethod()
    {
        LogTo.Information("Foo");
    }
}