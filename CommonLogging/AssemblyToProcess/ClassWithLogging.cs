using Anotar.CommonLogging;

public class ClassWithLogging
{
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
        LogTo.Debug(() => "TheMessage");
    }

    public void DebugStringParams()
    {
        LogTo.Debug("TheMessage {0}", 1);
    }

    public void DebugStringException()
    {
        LogTo.DebugException("TheMessage", new());
    }

    public void DebugStringExceptionFunc()
    {
        LogTo.DebugException(()=>"TheMessage", new());
    }

    public void DebugStringExceptionParams()
    {
        LogTo.DebugException("TheMessage", new(), 1);
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
        LogTo.InfoException("TheMessage", new());
    }

    public void InfoStringExceptionFunc()
    {
        LogTo.InfoException(()=>"TheMessage", new());
    }

    public void InfoStringExceptionParams()
    {
        LogTo.InfoException("TheMessage", new(), 1);
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
        LogTo.WarnException("TheMessage", new());
    }

    public void WarnStringExceptionFunc()
    {
        LogTo.WarnException(()=>"TheMessage", new());
    }

    public void WarnStringExceptionParams()
    {
        LogTo.WarnException("TheMessage", new(), 1);
    }

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
        LogTo.TraceException("TheMessage", new());
    }

    public void TraceStringExceptionFunc()
    {
        LogTo.TraceException(()=>"TheMessage", new());
    }

    public void TraceStringExceptionParams()
    {
        LogTo.TraceException("TheMessage", new(), 1);
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
        LogTo.ErrorException("TheMessage", new());
    }

    public void ErrorStringExceptionFunc()
    {
        LogTo.ErrorException(()=>"TheMessage", new());
    }

    public void ErrorStringExceptionParams()
    {
        LogTo.ErrorException("TheMessage", new(), 1);
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
        LogTo.FatalException("TheMessage", new());
    }

    public void FatalStringExceptionFunc()
    {
        LogTo.FatalException(()=>"TheMessage", new());
    }

    public void FatalStringExceptionParams()
    {
        LogTo.FatalException("TheMessage", new(), 1);
    }
}