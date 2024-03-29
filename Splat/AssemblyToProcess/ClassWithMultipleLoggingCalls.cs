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
        LogTo.DebugException("Debug", new());
        LogTo.InfoException("Info", new());
        LogTo.WarnException("Warn", new());
    }

    public void LogThrowLog(bool doThrow)
    {
        LogTo.Info("Doing something");

        if (doThrow)
        {
            throw new();
        }

        LogTo.Info("Doing something");
    }
}