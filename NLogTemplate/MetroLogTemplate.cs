using System;
using MetroLog;

public class MetroLogTemplate
{
    static ILogger Logger;

    static MetroLogTemplate()
    {
        Logger = LogManagerFactory.DefaultLogManager.GetLogger("MetroLogTemplate");
    }

    void StringMethod()
    {
        var theMessage = "TheMessage";
        Logger.Debug(string.Concat("MyMethod", theMessage));
    }

    void FormatMethod()
    {
        var format = "Hey {0}";
        var theMessage = string.Format(format, new object[] { 1 });
        Logger.Debug(string.Concat("MyMethod", theMessage));
    }

    void ExceptionMethod()
    {
        var theMessage = "TheMessage";
        var exception = new Exception();
        Logger.Debug(string.Concat("MyMethod", theMessage), exception);
    }

    void SimpleMethod()
    {
        Logger.Debug("MyMethod");
    }
}