using System.IO;
using NUnit.Framework;

[TestFixture]
public class Log4NetTests:BaseTests
{

    public Log4NetTests()
		: base(Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugLog4Net\Log4NetAssemblyToProcess.dll"))
    {
    }

}