using System;
using NLog;

public class NLogTemplate
{
    static Logger AnotarLogger;
    static NLogTemplate()
    {
       AnotarLogger =LogManager.GetCurrentClassLogger();
    }
    void MyMethod()
    {
        var theMessage = "TheMessage";
        var exception = new Exception();
        AnotarLogger.DebugException( theMessage,exception);
    }
}