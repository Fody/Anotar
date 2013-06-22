using System;
using Anotar.Custom;

public class ClassWithLogging
{
    public async void AsyncMethod()
    {
        System.Diagnostics.Trace.WriteLine("Foo");
    }
    public void Debug()
    {
        Log.Debug();
    }
    public void DebugString()
    {
        Log.Debug("TheMessage");
    }
    public void DebugStringParams()
    {
        Log.Debug("TheMessage {0}",1);
    }
    public void DebugStringException()
    {
        Log.Debug(new Exception(), "TheMessage");
    }
    public void Information()
    {
        Log.Information();
    }
    public void InformationString()
    {
        Log.Information("TheMessage");
    }
    public void InformationStringParams()
    {
        Log.Information("TheMessage {0}", 1);
    }
    public void InformationStringException()
    {
        Log.Information(new Exception(), "TheMessage");
    }
    public void Warning()
    {
        Log.Warning();
    }
    public void WarningString()
    {
        Log.Warning("TheMessage");
    }
    public void WarningStringParams()
    {
        Log.Warning("TheMessage {0}", 1);
    }
    public void WarningStringException()
    {
        Log.Warning(new Exception(), "TheMessage");
    }
    public void Error()
    {
        Log.Error();
    }
    public void ErrorString()
    {
        Log.Error("TheMessage");
    }
    public void ErrorStringParams()
    {
        Log.Error("TheMessage {0}", 1);
    }
    public void ErrorStringException()
    {
        Log.Error(new Exception(), "TheMessage");
    }
    public void Fatal()
    {
        Log.Fatal();
    }
    public void FatalString()
    {
        Log.Fatal("TheMessage");
    }
    public void FatalStringParams()
    {
        Log.Fatal("TheMessage {0}", 1);
    }
    public void FatalStringException()
    {
        Log.Fatal(new Exception(), "TheMessage");
    }
}