using System.Collections.Generic;
// ReSharper disable UnusedTypeParameter

public class LoggerFactory
{
    public static List<LogEntry> ErrorEntries = new List<LogEntry>();
    public static List<LogEntry> FatalEntries = new List<LogEntry>();
    public static List<LogEntry> DebugEntries = new List<LogEntry>();
    public static List<LogEntry> InformationEntries = new List<LogEntry>();
    public static List<LogEntry> WarningEntries = new List<LogEntry>();
    public static List<LogEntry> TraceEntries = new List<LogEntry>();

    public static Logger GetLogger<T>()
    {
        return new Logger
               {
                   ErrorEntries = ErrorEntries,
                   FatalEntries = FatalEntries,
                   DebugEntries = DebugEntries,
                   InformationEntries = InformationEntries,
                   WarningEntries = WarningEntries,
                   TraceEntries = TraceEntries,
               };
    }

    public static void Clear()
    {
        ErrorEntries.Clear();
        FatalEntries.Clear();
        DebugEntries.Clear();
        InformationEntries.Clear();
        WarningEntries.Clear();
        TraceEntries.Clear();
    }
}