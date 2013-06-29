using System;
using System.Diagnostics;
using Anotar.Log4Net;

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
        Log.DebugException("TheMessage",new Exception());
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
        Log.InfoException("TheMessage", new Exception());
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
        Log.WarnException("TheMessage", new Exception());
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
        Log.ErrorException("TheMessage", new Exception());
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
		Log.FatalException("TheMessage", new Exception());
    }
}