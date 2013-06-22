using System.Collections.Generic;

public class LoggerFactory
{

    public static List<LogEntry> Errors = new List<LogEntry>();
    public static List<LogEntry> Fatals = new List<LogEntry>();
    public static List<LogEntry> Debugs = new List<LogEntry>();
    public static List<LogEntry> Traces = new List<LogEntry>();
    public static List<LogEntry> Infos = new List<LogEntry>();
    public static List<LogEntry> Warns = new List<LogEntry>();

    public static Logger GetLogger<T>()
    {
        return new Logger
               {

                   Errors = Errors,
                   Fatals = Fatals,
                   Debugs = Debugs,
                   Traces = Traces,
                   Infos = Infos,
                   Warns = Warns,
               };
    }

    public static void Clear()
    {
        Errors.Clear();
        Fatals.Clear();
        Debugs.Clear();
        Traces.Clear();
        Infos.Clear();
        Warns.Clear();
    }
}