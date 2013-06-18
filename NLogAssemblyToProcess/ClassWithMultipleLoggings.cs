using System;
using Anotar.NLog;


public class ClassWithMultipleLoggings
{
    public void DoLog()
    {
        Log.Debug("Debug");
        Log.Info("Info");
        Log.Warn("Warn");
    }

    public void DoLog(object[] args)
    {
        Log.Debug("Debug", args);
        Log.Info("Info", args);
        Log.Warn("Warn", args);
    }
}
