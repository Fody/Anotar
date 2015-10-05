using System;
using Anotar.CommonLogging;
#pragma warning disable 1998

public class ClassWithException
{
    public async void AsyncMethod()
    {
        try
        {
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            TimeSpan.FromDays(1);
        }
        catch
        {
        }
    }

    public void Debug()
    {
        try
        {
            LogTo.Debug();
        }
        catch
        {
        }
    }

    public void DebugString()
    {
        try
        {
            LogTo.Debug("TheMessage");
        }
        catch
        {
        }
    }

    public void DebugStringParams()
    {
        try
        {
            LogTo.Debug("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void DebugStringException()
    {
        try
        {
            LogTo.DebugException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Trace()
    {
        try
        {
            LogTo.Trace();
        }
        catch
        {
        }
    }

    public void TraceString()
    {
        try
        {
            LogTo.Info("TheMessage");
        }
        catch
        {
        }
    }

    public void TraceStringParams()
    {
        try
        {
            LogTo.Trace("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void TraceStringException()
    {
        try
        {
            LogTo.TraceException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Info()
    {
        try
        {
            LogTo.Info();
        }
        catch
        {
        }
    }

    public void InfoString()
    {
        try
        {
            LogTo.Info("TheMessage");
        }
        catch
        {
        }
    }

    public void InfoStringParams()
    {
        try
        {
            LogTo.Info("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void InfoStringException()
    {
        try
        {
            LogTo.InfoException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Warn()
    {
        try
        {
            LogTo.Warn();
        }
        catch
        {
        }
    }

    public void WarnString()
    {
        try
        {
            LogTo.Warn("TheMessage");
        }
        catch
        {
        }
    }

    public void WarnStringParams()
    {
        try
        {
            LogTo.Warn("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void WarnStringException()
    {
        try
        {
            LogTo.WarnException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Error()
    {
        try
        {
            LogTo.Error();
        }
        catch
        {
        }
    }

    public void ErrorString()
    {
        try
        {
            LogTo.Error("TheMessage");
        }
        catch
        {
        }
    }

    public void ErrorStringParams()
    {
        try
        {
            LogTo.Error("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void ErrorStringException()
    {
        try
        {
            LogTo.ErrorException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Fatal()
    {
        try
        {
            LogTo.Fatal();
        }
        catch
        {
        }
    }

    public void FatalString()
    {
        try
        {
            LogTo.Fatal("TheMessage");
        }
        catch
        {
        }
    }

    public void FatalStringParams()
    {
        try
        {
            LogTo.Fatal("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void FatalStringException()
    {
        try
        {
            LogTo.FatalException("TheMessage", new Exception());
        }
        catch
        {
        }
    }
}