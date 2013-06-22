using System;
using System.Collections.Generic;

public class Logger
{
    public void Trace(string format, params object[] args)
    {
        Traces.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });
    }
    public void Trace(Exception exception, string format, params object[] args)
    {
        Traces.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }
    public bool IsTraceEnabled{get { return true; }}

    public void Debug(string format, params object[] args)
    {
        Debugs.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });   
    }
    public void Debug(Exception exception, string format, params object[] args)
    {
        Debugs.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }

    public bool IsDebugEnabled { get { return true; } }

    public void Information(string format, params object[] args)
    {
        Infos.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });
    }
    public void Information(Exception exception, string format, params object[] args)
    {
        Infos.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }
    public bool IsInformationEnabled { get { return true; } }

    public void Warn(string format, params object[] args)
    {
        Warns.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });
    }
    public void Warn(Exception exception, string format, params object[] args)
    {
        Warns.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }    
    public bool IsWarnEnabled { get { return true; } }

    public void Error(string format, params object[] args)
    {

        Errors.Add(new LogEntry
        {
            Format = format,
            Params = args,
        });
    }
    public void Error(Exception exception, string format, params object[] args)
    {
        Errors.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    } 
    public bool IsErrorEnabled { get { return true; } }

    public void Fatal(string format, params object[] args)
    {
        Fatals.Add(new LogEntry
                   {
                       Format = format,
                       Params = args
                   });
    }
    public void Fatal(Exception exception, string format, params object[] args)
    {

        Fatals.Add(new LogEntry
        {
            Format = format,
            Params = args,
            Exception = exception
        });
    }
    public bool IsFatalEnabled { get { return true; } }
    public List<LogEntry> Errors;
    public List<LogEntry> Fatals;
    public List<LogEntry> Debugs;
    public List<LogEntry> Traces;
    public List<LogEntry> Infos;
    public List<LogEntry> Warns;
}