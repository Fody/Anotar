using System;
using Anotar.Splat;

public class ClassWithMultipleLoggingCalls
{
    public void LogNoArgs()
    {
        LogTo.Debug("Debug");
        LogTo.Info("Info");
        LogTo.Warn("Warn");
    }

    public void LogWithArgs()
    {
        var args = new object[] { 1 };

        LogTo.Debug("Debug", args);
        LogTo.Info("Info", args);
        LogTo.Warn("Warn", args);
    }

    public void LogErrors()
    {
        LogTo.DebugException("Debug", new Exception());
        LogTo.InfoException("Info", new Exception());
        LogTo.WarnException("Warn", new Exception());
    }

    public void LogThrowLog(bool doThrow)
    {
        LogTo.Info("Doing something");

        if (doThrow)
        {
            throw new Exception();
        }

        LogTo.Info("Doing something");
    }
}