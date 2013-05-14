using System.Collections.Generic;
using Scalpel;
using Serilog.Events;

[Remove]
public static class LogEventExtensions
{
    public static string Value(this LogEvent logEvent, string property)
    {
//// ReSharper disable RedundantCast
        var readOnlyDictionary = (IReadOnlyDictionary<string, LogEventProperty>)logEvent.Properties;
//// ReSharper restore RedundantCast
        return (string) ((ScalarValue) readOnlyDictionary[property].Value).Value;
    }
}