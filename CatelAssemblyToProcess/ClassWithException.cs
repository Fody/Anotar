using System;
using Anotar.Catel;
#pragma warning disable 1998

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
            LogTo.Debug(new Exception(),"TheMessage");
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
            LogTo.Info(new Exception(), "TheMessage");
        }
        catch
        {
        }
    }

    public void Warn()
    {
        try
        {
            LogTo.Warning();
        }
        catch
        {
        }
    }

    public void WarnString()
    {
        try
        {
            LogTo.Warning("TheMessage");
        }
        catch
        {
        }
    }

    public void WarnStringParams()
    {
        try
        {
            LogTo.Warning("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void WarnStringException()
    {
        try
        {
            LogTo.Warning(new Exception(), "TheMessage");
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
            LogTo.Error(new Exception(),"TheMessage");
        }
        catch
        {
        }
    }
}