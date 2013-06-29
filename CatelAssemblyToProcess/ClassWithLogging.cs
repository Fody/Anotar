using System;
using Anotar.Catel;

public class ClassWithLogging
{
    public async void AsyncMethod()
    {
        System.Diagnostics.Trace.WriteLine("Foo");
    }
    public void Debug()
    {
        LogTo.Debug();
    }
    public void DebugString()
    {
        LogTo.Debug("TheMessage");
    }
    public void DebugStringParams()
    {
        LogTo.Debug("TheMessage {0}",1);
    }
    public void PassInString(string message)
    {
        LogTo.Debug(message, 1);
    }
    public void DebugStringException()
    {
        LogTo.Debug(new Exception(),"TheMessage");
    }
    public void Info()
    {
        LogTo.Info();
    }
    public void InfoString()
    {
        LogTo.Info("TheMessage");
    }
    public void InfoStringParams()
    {
        LogTo.Info("TheMessage {0}", 1);
    }
    public void InfoStringException()
    {
        LogTo.Info(new Exception(), "TheMessage");
    }
    public void Warn()
    {
        LogTo.Warning();
    }
    public void WarnString()
    {
        LogTo.Warning("TheMessage");
    }
    public void WarnStringParams()
    {
        LogTo.Warning("TheMessage {0}", 1);
    }
    public void WarnStringException()
    {
        LogTo.Warning(new Exception(), "TheMessage");
    }
    public void Error()
    {
        LogTo.Error();
    }
    public void ErrorString()
    {
        LogTo.Error("TheMessage");
    }
    public void ErrorStringParams()
    {
        LogTo.Error("TheMessage {0}", 1);
    }
    public void ErrorStringException()
    {
        LogTo.Error(new Exception(), "TheMessage");
    }
}