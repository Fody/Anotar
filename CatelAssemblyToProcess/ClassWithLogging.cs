using System;
using Anotar.Catel;

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
        LogTo.Debug(()=>"TheMessage");
    }

    public void DebugStringParams()
    {
        LogTo.Debug("TheMessage {0}", 1);
    }

    public void PassInString(string message)
    {
        LogTo.Debug(message, 1);
    }

    public void DebugStringException()
    {
        LogTo.Debug(new Exception(), "TheMessage");
    }

    public void DebugStringExceptionFunc()
    {
        LogTo.Debug(new Exception(),()=> "TheMessage");
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
        LogTo.Info(new Exception(), "TheMessage");
    }

    public void InfoStringExceptionFunc()
    {
        LogTo.Info(new Exception(), ()=>"TheMessage");
    }

    public bool IsWarningEnabled()
    {
        return LogTo.IsWarningEnabled;
    }

    public void Warning()
    {
        LogTo.Warning();
    }

    public void WarningString()
    {
        LogTo.Warning("TheMessage");
    }

    public void WarningStringFunc()
    {
        LogTo.Warning(()=>"TheMessage");
    }

    public void WarningStringParams()
    {
        LogTo.Warning("TheMessage {0}", 1);
    }

    public void WarningStringException()
    {
        LogTo.Warning(new Exception(), "TheMessage");
    }

    public void WarningStringExceptionFunc()
    {
        LogTo.Warning(new Exception(), ()=>"TheMessage");
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
        LogTo.Error(() => "TheMessage");
    }

    public void ErrorStringParams()
    {
        LogTo.Error("TheMessage {0}", 1);
    }

    public void ErrorStringException()
    {
        LogTo.Error(new Exception(), "TheMessage");
    }

    public void ErrorStringExceptionFunc()
    {
        LogTo.Error(new Exception(), () => "TheMessage");
    }
}