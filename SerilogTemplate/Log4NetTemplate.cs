using log4net;

public class Log4NetTemplate
{
    static ILog logger = LogManager.GetLogger("Log4NetTemplate");

    void Method()
    {
        logger.Debug("sdfsdf");
    }
}