using System;
using System.Collections.Generic;

public class Logger
{
    public void Trace(string format, params object[] args)
    {
        TraceEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });   
    }
    public void Trace(Exception exception, string format, params object[] args)
    {
        TraceEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }
    public bool IsTraceEnabled { get { return true; } }

    public void Debug(string format, params object[] args)
    {
        DebugEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });   
    }
    public void Debug(Exception exception, string format, params object[] args)
    {
        DebugEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }

    public bool IsDebugEnabled { get { return true; } }

    public void Information(string format, params object[] args)
    {
        InformationEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });
    }
    public void Information(Exception exception, string format, params object[] args)
    {
        InformationEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }
    public bool IsInformationEnabled { get { return true; } }

    public void Warning(string format, params object[] args)
    {
        WarningEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });
    }
    public void Warning(Exception exception, string format, params object[] args)
    {
        WarningEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }
    public bool IsWarningEnabled { get { return true; } }

    public void Error(string format, params object[] args)
    {

        ErrorEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });
    }
    public void Error(Exception exception, string format, params object[] args)
    {
        ErrorEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    } 
    public bool IsErrorEnabled { get { return true; } }

    public void Fatal(string format, params object[] args)
    {
        FatalEntries.Add(new LogEntry
                   {
                       Format = format,
                       Params = args
                   });
    }
    public void Fatal(Exception exception, string format, params object[] args)
    {
        FatalEntries.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }
    public bool IsFatalEnabled { get { return true; } }
    public List<LogEntry> ErrorEntries;
    public List<LogEntry> FatalEntries;
    public List<LogEntry> DebugEntries;
    public List<LogEntry> InformationEntries;
    public List<LogEntry> WarningEntries;
    public List<LogEntry> TraceEntries;
}