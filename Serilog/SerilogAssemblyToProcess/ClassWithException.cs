using System;
using System.Diagnostics;
using Anotar.Serilog;
#pragma warning disable 1998

public class ClassWithException
{
    public void Verbose()
    {
        try
        {
            LogTo.Verbose();
        }
        catch
        {
        }
    }

    public void VerboseString()
    {
        try
        {
            LogTo.Verbose("TheMessage");
        }
        catch
        {
        }
    }

    public void VerboseStringParams()
    {
        try
        {
            LogTo.Verbose("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void VerboseStringException()
    {
        try
        {
            LogTo.Verbose(new Exception(), "TheMessage");
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
            LogTo.Debug(new Exception(), "TheMessage");
        }
        catch
        {
        }
    }

    public void Info()
    {
        try
        {
            LogTo.Information();
        }
        catch
        {
        }
    }

    public void InfoString()
    {
        try
        {
            LogTo.Information("TheMessage");
        }
        catch
        {
        }
    }

    public void InfoStringParams()
    {
        try
        {
            LogTo.Information("TheMessage {0}", 1);
        }
        catch
        {
        }
    }

    public void InfoStringException()
    {
        try
        {
            LogTo.Information(new Exception(), "TheMessage");
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
            LogTo.Error(new Exception(), "TheMessage");
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
            LogTo.Fatal(new Exception(), "TheMessage");
        }
        catch
        {
        }
    }

    public async void AsyncMethod()
    {
        try
        {
            Trace.WriteLine("Foo");
        }
        catch
        {
        }
    }
}