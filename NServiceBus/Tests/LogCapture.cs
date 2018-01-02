using System;
using System.Collections.Generic;
using NServiceBus.Logging;

public class LogCapture : ILoggerFactory, ILog
{
    List<string> fatals;
    List<string> errors;
    List<string> debugs;
    List<string> infos;
    List<string> warns;

    public LogCapture(List<string> fatals, List<string> errors, List<string> debugs, List<string> infos, List<string> warns)
    {
        this.fatals = fatals;
        this.errors = errors;
        this.debugs = debugs;
        this.infos = infos;
        this.warns = warns;
    }

    public ILog GetLogger(Type type)
    {
        return this;
    }

    public ILog GetLogger(string name)
    {
        return this;
    }

    public void Debug(string message)
    {
        debugs.Add(message);
    }

    public void Debug(string message, Exception exception)
    {
        debugs.Add(message);
    }

    public void DebugFormat(string format, params object[] args)
    {
        debugs.Add(string.Format(format,args));
    }

    public void Info(string message)
    {
        infos.Add(message);
    }

    public void Info(string message, Exception exception)
    {
        infos.Add(message);
    }

    public void InfoFormat(string format, params object[] args)
    {
        infos.Add(string.Format(format, args));
    }

    public void Warn(string message)
    {
        warns.Add(message);
    }

    public void Warn(string message, Exception exception)
    {
        warns.Add(message);
    }

    public void WarnFormat(string format, params object[] args)
    {
        warns.Add(string.Format(format, args));
    }

    public void Error(string message)
    {
        errors.Add(message);
    }

    public void Error(string message, Exception exception)
    {
        errors.Add(message);
    }

    public void ErrorFormat(string format, params object[] args)
    {
        errors.Add(string.Format(format, args));
    }

    public void Fatal(string message)
    {
        fatals.Add(message);
    }

    public void Fatal(string message, Exception exception)
    {
        fatals.Add(message);
    }

    public void FatalFormat(string format, params object[] args)
    {
        fatals.Add(string.Format(format, args));
    }

    public bool IsDebugEnabled => true;
    public bool IsInfoEnabled => true;
    public bool IsWarnEnabled => true;
    public bool IsErrorEnabled => true;
    public bool IsFatalEnabled => true;
}