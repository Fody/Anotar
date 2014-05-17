using Scalpel;
using Serilog.Events;

[Remove]
public static class LogEventExtensions
{
    public static string MethodName(this LogEvent logEvent)
    {
        var logEventPropertyValue = (ScalarValue)logEvent.Properties["MethodName"];
        return (string)logEventPropertyValue.Value;
    }
    public static int LineNumber(this LogEvent logEvent)
    {
        var logEventPropertyValue = (ScalarValue)logEvent.Properties["LineNumber"];
        return (int)logEventPropertyValue.Value;
    }
}