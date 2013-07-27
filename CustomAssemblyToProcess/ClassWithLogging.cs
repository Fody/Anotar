using System;
using Anotar.Custom;

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
        LogTo.Debug("TheMessage {0}",1);
    }
    public void DebugStringException()
    {
        LogTo.Debug(new Exception(), "TheMessage");
    }
    public void Information()
    {
        LogTo.Information();
    }
    public void InformationString()
    {
        LogTo.Information("TheMessage");
    }
    public void InformationStringParams()
    {
        LogTo.Information("TheMessage {0}", 1);
    }
    public void InformationStringException()
    {
        LogTo.Information(new Exception(), "TheMessage");
    }
    public void Warning()
    {
        LogTo.Warning();
    }
    public void WarningString()
    {
        LogTo.Warning("TheMessage");
    }
    public void WarningStringParams()
    {
        LogTo.Warning("TheMessage {0}", 1);
    }
    public void WarningStringException()
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