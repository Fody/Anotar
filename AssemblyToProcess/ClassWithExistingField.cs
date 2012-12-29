using System;
using Anotar;
using NLog;

public class ClassWithExistingField
{
    static Logger existingLogger;

    static ClassWithExistingField()
    {
        existingLogger = LogManager.GetLogger("OnException");
    }

    public void Debug()
    {
        Log.Debug();
    }
}