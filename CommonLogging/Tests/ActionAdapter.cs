using System;
using System.Collections.Generic;
using Common.Logging;

public class ActionAdapter : ILoggerFactoryAdapter
{
    public List<LogEvent> Errors = new();
    public List<LogEvent> Debugs = new();
    public List<LogEvent> Informations = new();
    public List<LogEvent> Traces = new();
    public List<LogEvent> Warnings = new();
    public List<LogEvent> Fatals = new();

    public ILog GetLogger(Type type)
    {
        return new ActionLog(this);
    }

    public ILog GetLogger(string name)
    {
        return new ActionLog(this);
    }
}