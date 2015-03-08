using System;
using NServiceBus.Logging;
using Scalpel;

[Remove]
public class LogCapture : ILoggerFactory, ILog
{
    NServiceBusTests test;

    public LogCapture(NServiceBusTests test)
    {
        this.test = test;
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
        test.Debugs.Add(message);
    }

    public void Debug(string message, Exception exception)
    {
        test.Debugs.Add(message);
    }

    public void DebugFormat(string format, params object[] args)
    {
        test.Debugs.Add(string.Format(format,args));
    }

    public void Info(string message)
    {
        test.Infos.Add(message);
    }

    public void Info(string message, Exception exception)
    {
        test.Infos.Add(message);
    }

    public void InfoFormat(string format, params object[] args)
    {
        test.Infos.Add(string.Format(format, args));
    }

    public void Warn(string message)
    {
        test.Warns.Add(message);
    }

    public void Warn(string message, Exception exception)
    {
        test.Warns.Add(message);
    }

    public void WarnFormat(string format, params object[] args)
    {
        test.Warns.Add(string.Format(format, args));
    }

    public void Error(string message)
    {
        test.Errors.Add(message);
    }

    public void Error(string message, Exception exception)
    {
        test.Errors.Add(message);
    }

    public void ErrorFormat(string format, params object[] args)
    {
        test.Errors.Add(string.Format(format, args));
    }

    public void Fatal(string message)
    {
        test.Fatals.Add(message);
    }

    public void Fatal(string message, Exception exception)
    {
        test.Fatals.Add(message);
    }

    public void FatalFormat(string format, params object[] args)
    {
        test.Fatals.Add(string.Format(format, args));
    }

    public bool IsDebugEnabled
    {
        get { return true;}
    }

    public bool IsInfoEnabled
    {
        get { return true; }
    }

    public bool IsWarnEnabled
    {
        get { return true; }
    }

    public bool IsErrorEnabled
    {
        get { return true; }
    }

    public bool IsFatalEnabled
    {
        get { return true; }
    }
}