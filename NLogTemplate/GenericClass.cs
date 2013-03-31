using NLog;

public class GenericClass<T>
{
    private static Logger AnotarLogger;

    static GenericClass()
    {
        AnotarLogger = LogManager.GetLogger("GenericClass`1");
    }

    public void Debug()
    {
        AnotarLogger.Debug("Method: 'System.Void GenericClass`1::Debug()'. Line: ~8. ");
    }
}