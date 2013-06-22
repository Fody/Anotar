using System;
using Anotar.Custom;


public class ClassWithMultipleLoggings
{
    public void LogNoArgs()
    {
        Log.Debug("Debug");
        Log.Information("Info");
        Log.Warning("Warn");
    }

    public void LogWithArgs()
    {
        var args = new object[] { 1 };

        Log.Debug("Debug", args);
        Log.Information("Info", args);
        Log.Warning("Warn", args);
    }

    public void LogErrors()
    {
        Log.Debug(new Exception(), "Debug");
        Log.Information(new Exception(), "Info");
        Log.Warning(new Exception(), "Warn");
    }
}