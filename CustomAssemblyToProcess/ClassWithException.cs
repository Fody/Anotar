using System;
using Anotar.Custom;

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
        Log.Debug("TheMessage {0}",1);
        }
        catch
        {
        }
    }
    public void DebugStringException()
    {
        try
        {
        Log.Debug(new Exception(), "TheMessage");
        }
        catch
        {
        }
    }
    public void Information()
    {
        try
        {
        Log.Information();
        }
        catch
        {
        }
    }
    public void InformationString()
    {
        try
        {
        Log.Information("TheMessage");
        }
        catch
        {
        }
    }
    public void InformationStringParams()
    {
        try
        {
        Log.Information("TheMessage {0}", 1);
        }
        catch
        {
        }
    }
    public void InformationStringException()
    {
        try
        {
        Log.Information(new Exception(), "TheMessage");
        }
        catch
        {
        }
    }
    public void Warning()
    {
        try
        {
        Log.Warning();
        }
        catch
        {
        }
    }
    public void WarningString()
    {
        try
        {
            Log.Warning("TheMessage");
        }
        catch
        {
        }
    }
    public void WarningStringParams()
    {
        try
        {
        Log.Warning("TheMessage {0}", 1);
        }
        catch
        {
        }
    }
    public void WarningStringException()
    {
        try
        {
        Log.Warning(new Exception(), "TheMessage");
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
        Log.Error(new Exception(), "TheMessage");
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
        Log.Fatal(new Exception(), "TheMessage");
        }
        catch
        {
        }
    }
}