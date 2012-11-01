using Anotar;

namespace Before
{
    public class MyClass
    {
        void MyMethod()
        {
            Log.Debug("TheMessage");
        }
    }
}

namespace AfterNlog
{
    public class MyClass
    {
        static NLog.Logger logger = NLog.LogManager.GetLogger("MyClass");

        void MyMethod()
        {
            logger.Debug("Method: MyMethod. Line: ~12. TheMessage");
        }
    }
}
namespace AfterLog4Net
{
    public class MyClass
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger("MyClass");

        void MyMethod()
        {
            logger.Debug("Method: MyMethod. Line: ~12. TheMessage");
        }
    }
}