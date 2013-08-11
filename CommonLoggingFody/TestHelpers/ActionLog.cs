using System;
using Common.Logging;
using Scalpel;

[Remove]
public class ActionLog : ILog
{
    ActionAdapter actionAdapter;

    public ActionLog(ActionAdapter actionAdapter)
    {
        this.actionAdapter = actionAdapter;
    }

    public void Trace(object message)
    {
        throw new NotImplementedException();
    }

    public void Trace(object message, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void TraceFormat(string format, params object[] args)
    {
        actionAdapter.Traces.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args,
                                 });
    }

    public void TraceFormat(string format, Exception exception, params object[] args)
    {
        actionAdapter.Traces.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args,
                                     Exception = exception
                                 });
    }

    public void TraceFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void TraceFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void Trace(Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Trace(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Trace(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Debug(object message)
    {
        throw new NotImplementedException();
    }

    public void Debug(object message, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void DebugFormat(string format, params object[] args)
    {
        actionAdapter.Debugs.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args
                                 });

    }

    public void DebugFormat(string format, Exception exception, params object[] args)
    {
        actionAdapter.Debugs.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args,
                                     Exception = exception
                                 });
    }

    public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void Debug(Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Debug(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Debug(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Info(object message)
    {
        throw new NotImplementedException();
    }

    public void Info(object message, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void InfoFormat(string format, params object[] args)
    {
        actionAdapter.Infos.Add(new LogEvent
                                {
                                    Format = format,
                                    Args = args
                                });
    }

    public void InfoFormat(string format, Exception exception, params object[] args)
    {
        actionAdapter.Infos.Add(new LogEvent
                                {
                                    Format = format,
                                    Args = args,
                                    Exception = exception
                                });
    }

    public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void Info(Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Info(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Info(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Warn(object message)
    {
        throw new NotImplementedException();
    }

    public void Warn(object message, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void WarnFormat(string format, params object[] args)
    {
        actionAdapter.Warns.Add(new LogEvent
                                {
                                    Format = format,
                                    Args = args
                                });
    }

    public void WarnFormat(string format, Exception exception, params object[] args)
    {
        actionAdapter.Warns.Add(new LogEvent
                                {
                                    Format = format,
                                    Args = args,
                                    Exception = exception
                                });
    }

    public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void Warn(Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Warn(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Warn(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Error(object message)
    {
        throw new NotImplementedException();
    }

    public void Error(object message, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void ErrorFormat(string format, params object[] args)
    {
        actionAdapter.Errors.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args
                                 });
    }

    public void ErrorFormat(string format, Exception exception, params object[] args)
    {
        actionAdapter.Errors.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args,
                                     Exception = exception
                                 });
    }

    public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void Error(Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Error(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Error(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Fatal(object message)
    {
        throw new NotImplementedException();
    }

    public void Fatal(object message, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void FatalFormat(string format, params object[] args)
    {
        actionAdapter.Fatals.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args,
                                 });
    }

    public void FatalFormat(string format, Exception exception, params object[] args)
    {
        actionAdapter.Fatals.Add(new LogEvent
                                 {
                                     Format = format,
                                     Args = args,
                                     Exception = exception
                                 });
    }

    public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
    {
        throw new NotImplementedException();
    }

    public void Fatal(Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Fatal(Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback)
    {
        throw new NotImplementedException();
    }

    public void Fatal(IFormatProvider formatProvider, Action<FormatMessageHandler> formatMessageCallback, Exception exception)
    {
        throw new NotImplementedException();
    }

    public bool IsTraceEnabled { get{return true;}}
    public bool IsDebugEnabled { get { return true; } }
    public bool IsErrorEnabled { get { return true; } }
    public bool IsFatalEnabled { get { return true; } }
    public bool IsInfoEnabled { get { return true; } }
    public bool IsWarnEnabled { get { return true; } }
}