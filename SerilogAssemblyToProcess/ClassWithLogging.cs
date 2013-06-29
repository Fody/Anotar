using System;
using Anotar.Serilog;

public class ClassWithLogging
{
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
        LogTo.Debug("TheMessage {0}", 1);
    }
    public void DebugStringException()
    {
        LogTo.Debug(new Exception(), "TheMessage");
    }
    public void Info()
    {
        LogTo.Information();
    }
    public void InfoString()
    {
        LogTo.Information("TheMessage");
    }
    public void InfoStringParams()
    {
        LogTo.Information("TheMessage {0}", 1);
    }
    public void InfoStringException()
    {
        LogTo.Information(new Exception(), "TheMessage");
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
    public void Fatal()
    {
        LogTo.Fatal();
    }
	public void FatalString()
    {
        LogTo.Fatal("TheMessage");
    }
	public void FatalStringParams()
    {
        LogTo.Fatal("TheMessage {0}", 1);
    }
	public void FatalStringException()
    {
        LogTo.Fatal(new Exception(), "TheMessage");
    }
}