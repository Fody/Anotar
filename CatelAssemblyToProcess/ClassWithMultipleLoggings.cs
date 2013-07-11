using System;
using Anotar.Catel;


public class ClassWithMultipleLoggings
{
    public void LogNoArgs()
    {
        LogTo.Debug("Debug");
        LogTo.Info("Info");
        LogTo.Warning("Warn");
    }

    public void LogWithArgs()
    {
        var args = new object[] { 1 };

        LogTo.Debug("Debug", args);
        LogTo.Info("Info", args);
        LogTo.Warning("Warn", args);
    }

    public void LogErrors()
    {
        LogTo.Debug(new Exception(),"Debug");
        LogTo.Info(new Exception(),"Info");
        LogTo.Warning(new Exception(), "Warn");
    }

    public void LogThrowLog(bool dothrow)
    {
        LogTo.Info("Doing something");

        if (dothrow)
            throw new Exception();

        LogTo.Info("Doing something");
    }
}