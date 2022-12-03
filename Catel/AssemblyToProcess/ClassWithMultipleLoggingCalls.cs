using Anotar.Catel;

public class ClassWithMultipleLoggingCalls
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
        LogTo.Debug(new(),"Debug");
        LogTo.Info(new(),"Info");
        LogTo.Warning(new(), "Warn");
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