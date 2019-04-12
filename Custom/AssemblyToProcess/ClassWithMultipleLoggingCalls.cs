using System;
using Anotar.Custom;

public class ClassWithMultipleLoggingCalls
{
    public void LogNoArgs()
    {
        LogTo.Debug("Debug");
        LogTo.Information("Info");
        LogTo.Warning("Warn");
    }

    public void LogWithArgs()
    {
        var args = new object[] { 1 };

        LogTo.Debug("Debug", args);
        LogTo.Information("Info", args);
        LogTo.Warning("Warn", args);
    }

    public void LogErrors()
    {
        LogTo.Debug(new Exception(), "Debug");
        LogTo.Information(new Exception(), "Info");
        LogTo.Warning(new Exception(), "Warn");
    }

    public void LogThrowLog(bool doThrow)
    {
        LogTo.Information("Doing something");

        if (doThrow)
        {
            throw new Exception();
        }

        LogTo.Information("Doing something");
    }
}