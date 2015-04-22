using System;
using Anotar.NLog;

public class ClassWithLogging
{
    public bool IsTraceEnabled()
    {
        return LogTo.IsTraceEnabled;
    }

    public void Trace()
    {
        LogTo.Trace();
    }

    public void TraceString()
    {
        LogTo.Trace("TheMessage");
    }

    public void TraceStringFunc()
    {
        LogTo.Trace(()=>"TheMessage");
    }

    public void TraceStringParams()
    {
        LogTo.Trace("TheMessage {0}", 1);
    }

    public void TraceStringException()
    {
        LogTo.TraceException("TheMessage", new Exception());
    }

    public void TraceStringExceptionFunc()
    {
        LogTo.TraceException(()=>"TheMessage", new Exception());
    }

    public bool IsDebugEnabled()
    {
        return LogTo.IsDebugEnabled;
    }

    public void Debug()
    {
        LogTo.Debug();
    }

    public void DebugString()
    {
        LogTo.Debug("TheMessage");
    }

    public void DebugStringFunc()
    {
        LogTo.Debug(()=>"TheMessage");
    }

    public void DebugStringParams()
    {
        LogTo.Debug("TheMessage {0}", 1);
    }

    public void DebugStringException()
    {
        LogTo.DebugException("TheMessage", new Exception());
    }

    public void DebugStringExceptionFunc()
    {
        LogTo.DebugException(()=>"TheMessage", new Exception());
    }

    public bool IsInfoEnabled()
    {
        return LogTo.IsInfoEnabled;
    }

    public void Info()
    {
        LogTo.Info();
    }

    public void InfoString()
    {
        LogTo.Info("TheMessage");
    }

    public void InfoStringFunc()
    {
        LogTo.Info(()=>"TheMessage");
    }

    public void InfoStringParams()
    {
        LogTo.Info("TheMessage {0}", 1);
    }

    public void InfoStringException()
    {
        LogTo.InfoException("TheMessage", new Exception());
    }

    public void InfoStringExceptionFunc()
    {
        LogTo.InfoException(()=>"TheMessage", new Exception());
    }

    public bool IsWarnEnabled()
    {
        return LogTo.IsWarnEnabled;
    }

    public void Warn()
    {
        LogTo.Warn();
    }

    public void WarnString()
    {
        LogTo.Warn("TheMessage");
    }

    public void WarnStringFunc()
    {
        LogTo.Warn(()=>"TheMessage");
    }

    public void WarnStringParams()
    {
        LogTo.Warn("TheMessage {0}", 1);
    }

    public void WarnStringException()
    {
        LogTo.WarnException("TheMessage", new Exception());
    }

    public void WarnStringExceptionFunc()
    {
        LogTo.WarnException(()=>"TheMessage", new Exception());
    }

    public bool IsErrorEnabled()
    {
        return LogTo.IsErrorEnabled;
    }

    public void Error()
    {
        LogTo.Error();
    }

    public void ErrorString()
    {
        LogTo.Error("TheMessage");
    }

    public void ErrorStringFunc()
    {
        LogTo.Error(()=>"TheMessage");
    }

    public void ErrorStringParams()
    {
        LogTo.Error("TheMessage {0}", 1);
    }

    public void ErrorStringException()
    {
        LogTo.ErrorException("TheMessage", new Exception());
    }

    public void ErrorStringExceptionFunc()
    {
        LogTo.ErrorException(()=>"TheMessage", new Exception());
    }

    public bool IsFatalEnabled()
    {
        return LogTo.IsFatalEnabled;
    }

    public void Fatal()
    {
        LogTo.Fatal();
    }

    public void FatalString()
    {
        LogTo.Fatal("TheMessage");
    }

    public void FatalStringFunc()
    {
        LogTo.Fatal(()=>"TheMessage");
    }

    public void FatalStringParams()
    {
        LogTo.Fatal("TheMessage {0}", 1);
    }

    public void FatalStringException()
    {
        LogTo.FatalException("TheMessage", new Exception());
    }

    public void FatalStringExceptionFunc()
    {
        LogTo.FatalException(()=>"TheMessage", new Exception());
    }
}