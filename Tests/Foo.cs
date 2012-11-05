using MetroLog;
using NUnit.Framework;

[TestFixture]
[Ignore]
public class Foo
{
    private static ILogger AnotarLogger = LogManagerFactory.DefaultLogManager.GetLogger("ClassWithLogging", null);


    [Test]
    public void Debug()
    {
        AnotarLogger.Debug("Method: Debug. Line: ~8. ", (object[])null);
    }


}