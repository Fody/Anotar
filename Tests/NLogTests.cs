using System.IO;
using NUnit.Framework;

[TestFixture]
public class NLogTests:BaseTests
{

    public NLogTests()
        : base(Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\DebugNlog\AssemblyToProcessNLog.dll"))
    {
    }


}