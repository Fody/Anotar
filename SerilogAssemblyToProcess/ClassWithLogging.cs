using System;
using Anotar.Serilog;

public class ClassWithLogging
{
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
    public void Info()
    {
        Log.Info();
    }
    public void InfoString()
    {
        Log.Info("TheMessage");
    }
    public void InfoStringParams()
    {
        Log.Info("TheMessage {0}", 1);
    }
    public void InfoStringException()
    {
        Log.Info(new Exception(), "TheMessage");
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
        Log.Warn(new Exception(), "TheMessage");
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