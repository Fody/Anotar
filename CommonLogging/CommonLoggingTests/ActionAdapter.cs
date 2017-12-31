using System;
using System.Collections.Generic;
using Common.Logging;

public class ActionAdapter : ILoggerFactoryAdapter
{
    public List<LogEvent> Errors = new List<LogEvent>();
    public List<LogEvent> Debugs = new List<LogEvent>();
    public List<LogEvent> Informations = new List<LogEvent>();
    public List<LogEvent> Traces = new List<LogEvent>();
    public List<LogEvent> Warnings = new List<LogEvent>();
    public List<LogEvent> Fatals = new List<LogEvent>();

    public ILog GetLogger(Type type)
    {
        return new ActionLog(this);
    }

    public ILog GetLogger(string name)
    {
        return new ActionLog(this);
    }
}