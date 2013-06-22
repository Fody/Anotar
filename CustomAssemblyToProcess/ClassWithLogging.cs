using System;
using Anotar.Custom;

public class ClassWithLogging
{
    public async void AsyncMethod()
    {
        System.Diagnostics.Trace.WriteLine("Foo");
    }
    public void Trace()
    {
        Log.Trace();
    }
    public void TraceString()
    {
        Log.Trace("TheMessage");
    }
    public void TraceStringParams()
    {
        Log.Trace("TheMessage {0}", 1);
    }
    public void TraceStringException()
    {
        Log.Trace(new Exception(), "TheMessage");
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
    public void Warn()
    {
        Log.Warn();
    }
    public void WarnString()
    {
        Log.Warn("TheMessage");
    }
    public void WarnStringParams()
    {
        Log.Warn("TheMessage {0}", 1);
    }
    public void WarnStringException()
    {
        Log.Warn(new Exception(),"TheMessage");
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