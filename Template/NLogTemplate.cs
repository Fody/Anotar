using System;
using NLog;

public class NLogTemplate
{
    static Logger AnotarLogger;
    static NLogTemplate()
    {
       AnotarLogger =LogManager.GetCurrentClassLogger();
    }
    void StringMethod()
    {
        var theMessage = "TheMessage";
        AnotarLogger.Debug( string.Concat("MyMethod", theMessage));
    }
    void FormatMethod()
    {
        var format = "Hey {0}";
        var theMessage = string.Format(format, new object[] {1});
        AnotarLogger.Debug( string.Concat("MyMethod", theMessage));
    }
    void ExceptionMethod()
    {
        var theMessage = "TheMessage";
        var exception = new Exception();
        AnotarLogger.Debug(string.Concat("MyMethod", theMessage), exception);
    }
    void SimpleMethod()
    {
        AnotarLogger.Debug("MyMethod");
    }
}