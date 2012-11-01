using System;
using Anotar;

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
        Log.Debug("TheMessage",new Exception());
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
        Log.Info("TheMessage", new Exception());
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
        Log.Warn("TheMessage", new Exception());
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
        Log.Error("TheMessage", new Exception());
    }
}