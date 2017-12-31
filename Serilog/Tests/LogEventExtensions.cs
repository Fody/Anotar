using Serilog.Events;

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
    public static string SourceContext(this LogEvent logEvent)
    {
        var logEventPropertyValue = (ScalarValue)logEvent.Properties["SourceContext"];
        return (string)logEventPropertyValue.Value;
    }
}