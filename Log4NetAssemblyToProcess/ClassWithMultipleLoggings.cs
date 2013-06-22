using System;
using Anotar.Log4Net;


public class ClassWithMultipleLoggings
{
    public void LogNoArgs()
    {
        Log.Debug("Debug");
        Log.Info("Info");
        Log.Warn("Warn");
    }

    public void LogWithArgs()
    {
        var args = new object[] { 1 };

        Log.Debug("Debug", args);
        Log.Info("Info", args);
        Log.Warn("Warn", args);
    }

    public void LogErrors()
    {
        Log.DebugException("Debug", new Exception());
        Log.InfoException("Info", new Exception());
        Log.WarnException("Warn", new Exception());
    }
}
