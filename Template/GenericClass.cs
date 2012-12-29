using NLog;

public class GenericClass<T>
{
    private static Logger AnotarLogger;

    static GenericClass()
    {
        GenericClass<T>.AnotarLogger = LogManager.GetLogger("GenericClass`1");
    }

    public GenericClass()
    {
    }

    public void Debug()
    {
        GenericClass<T>.AnotarLogger.Debug("Method: 'System.Void GenericClass`1::Debug()'. Line: ~8. ");
    }
}