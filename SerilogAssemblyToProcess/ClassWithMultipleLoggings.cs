using System;
using Anotar.Serilog;


public class ClassWithMultipleLoggings
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

    public void LogThrowLog(bool dothrow)
    {
        LogTo.Information("Doing something");

        if (dothrow)
            throw new Exception();

        LogTo.Information("Doing something");
    }
}
