using System;
using Anotar.NLog;

public class ClassWithException
{
    public async void AsyncMethod()
    {
        try
        {
            System.Diagnostics.Trace.WriteLine("Foo");
        }
        catch
        {
        }
    }

    public void Trace()
    {
        try
        {
            Log.Trace();
        }
        catch
        {
        }
    }

    public void TraceString()
    {
        try
        {
            Log.Trace("TheMessage");
        }
        catch
        {
        }
    }

    public void TraceStringParams()
    {
        try
        {
            Log.Trace("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void TraceStringException()
    {
        try
        {
            Log.TraceException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Debug()
    {
        try
        {
            Log.Debug();
        }
        catch
        {
        }
    }

    public void DebugString()
    {
        try
        {
            Log.Debug("TheMessage");
        }
        catch
        {
        }
    }

    public void DebugStringParams()
    {
        try
        {
            Log.Debug("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void DebugStringException()
    {
        try
        {
            Log.DebugException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Info()
    {
        try
        {
            Log.Info();
        }
        catch
        {
        }
    }

    public void InfoString()
    {
        try
        {
            Log.Info("TheMessage");
        }
        catch
        {
        }
    }

    public void InfoStringParams()
    {
        try
        {
            Log.Info("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void InfoStringException()
    {
        try
        {
            Log.InfoException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Warn()
    {
        try
        {
            Log.Warn();
        }
        catch
        {
        }
    }

    public void WarnString()
    {
        try
        {
            Log.Warn("TheMessage");
        }
        catch
        {
        }
    }

    public void WarnStringParams()
    {
        try
        {
            Log.Warn("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void WarnStringException()
    {
        try
        {
            Log.WarnException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Error()
    {
        try
        {
            Log.Error();
        }
        catch
        {
        }
    }

    public void ErrorString()
    {
        try
        {
            Log.Error("TheMessage");
        }
        catch
        {
        }
    }

    public void ErrorStringParams()
    {
        try
        {
            Log.Error("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void ErrorStringException()
    {
        try
        {
            Log.ErrorException("TheMessage", new Exception());
        }
        catch
        {
        }
    }

    public void Fatal()
    {
        try
        {
            Log.Fatal();
        }
        catch
        {
        }
    }

    public void FatalString()
    {
        try
        {
            Log.Fatal("TheMessage");
        }
        catch
        {
        }
    }

    public void FatalStringParams()
    {
        try
        {
            Log.Fatal("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void FatalStringException()
    {
        try
        {
            Log.FatalException("TheMessage", new Exception());
        }
        catch
        {
        }
    }
}