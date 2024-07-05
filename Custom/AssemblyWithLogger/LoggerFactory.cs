
// ReSharper disable UnusedTypeParameter

public class LoggerFactory
{
    public static List<LogEntry> ErrorEntries = new();
    public static List<LogEntry> FatalEntries = new();
    public static List<LogEntry> DebugEntries = new();
    public static List<LogEntry> InformationEntries = new();
    public static List<LogEntry> WarningEntries = new();
    public static List<LogEntry> TraceEntries = new();

    public static Logger GetLogger<T>()
    {
        return new()
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